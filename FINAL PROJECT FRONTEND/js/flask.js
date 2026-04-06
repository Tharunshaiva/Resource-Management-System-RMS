(function($) {
    const EMPLOYEE_API = 'https://localhost:7230/api/Employee/GetAllEmployees';
    const BENCH_API = 'http://127.0.0.1:5001/bench-probability';

    let employees = []; // array of employee objects
    let selectedEmployee = null;

    // Utility: show transient text
    function showMessage(text, isError = true) {
        const $m = $('#message');
        $m.text(text).css('color', isError ? '#c92a2a' : '#1b5e20').show();
        setTimeout(() => $m.fadeOut(400), 4000);
    }

    function clearResult() {
        $('#resultOutput').text('No result yet');
        $('#resultDetails').text('');
    }

    // Build a custom suggestions list (we'll use a styled dropdown instead of browser datalist)
    function buildSuggestions() {
        const $container = $('#suggestionList').empty().hide();

        employees.forEach((emp, idx) => {
            const $item = $(`<div class="suggestion-item" data-index="${idx}" role="option">${escapeHtml(emp.fullName)}</div>`);
            $container.append($item);
        });
    }

    // Basic HTML escape for safety
    function escapeHtml(str) {
        if (!str) return '';
        return str.replace(/[&"'<>]/g, function(m){ return ({'&':'&amp;','"':'&quot;','\'':'&#39;','<':'&lt;','>':'&gt;'})[m]; });
    }

    // Find and set selected employee when user picks a name
    function setSelectedByName(name) {
        selectedEmployee = null;
        if (!name || name.trim().length === 0) {
            $('#selName').text('— none selected —');
            $('#selMeta').text('Primary skill • Experience');
            $('#btnAssess').prop('disabled', true);
            return;
        }

        const found = employees.find(e => (e.fullName||'').toLowerCase() === name.toLowerCase());
        if (!found) {
            // not a match — keep disabled, but allow typing
            $('#selName').text('Not found (try exact name from suggestions)');
            $('#selMeta').text('');
            $('#btnAssess').prop('disabled', true);
            return;
        }

        selectedEmployee = found;
        const skillText = Array.isArray(found.skills) && found.skills.length ? found.skills.join(', ') : '(no skills)';
        const exp = (found.experienceInYears != null) ? found.experienceInYears : '(unknown)';

        $('#selName').text(found.fullName + ` • ${found.designation || '—'}`);
        $('#selMeta').text(`${skillText} • ${exp} years`);
        $('#btnAssess').prop('disabled', false);
        clearResult();
    }

    // On click Assess -> POST to bench API
    function assessBench() {
        if (!selectedEmployee) {
            showMessage('Please select a valid employee from the suggestions.');
            return;
        }

        let skill = '';
        if (Array.isArray(selectedEmployee.skills) && selectedEmployee.skills.length) {
            // Send full array of skills (backend now accepts array or string)
            skill = selectedEmployee.skills.slice();
        } else {
            // fallback to skillIDs array if any, or empty string/array
            if (Array.isArray(selectedEmployee.skillIDs) && selectedEmployee.skillIDs.length) {
                skill = selectedEmployee.skillIDs.slice();
            } else {
                skill = '';
            }
        }

        // experience must be numeric (float)
        let experience = parseFloat(selectedEmployee.experienceInYears || 0);

        const payload = {
            skill: skill,
            experience: experience
        };

        $('#mainSpinner').show();
        $('#btnAssess').prop('disabled', true);

        $.ajax({
            url: BENCH_API,
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(payload),
            timeout: 8000
        })
        .done((resp) => {
            // Expected response: { "bench_probability": "%", "success": true }
            if (resp && resp.success) {
                const p = resp.bench_probability || resp.benchProbability || resp.probability || 'N/A';
                $('#resultOutput').text(typeof p === 'string' ? p : String(p));
                    // Format skills for display
                    const skillDisplay = Array.isArray(skill) ? skill.join(', ') : (skill || '(none)');
                    const details = `Skills: ${skillDisplay} • Experience: ${experience}`;
                $('#resultDetails').text(details);
            } else {
                showMessage('The bench API returned an error — check server logs.');
            }
        })
        .fail((xhr, status, err) => {
            if (status === 'timeout') {
                showMessage('Request timed out — the bench API may be down or slow.');
            } else {
                const msg = xhr && xhr.responseJSON && xhr.responseJSON.message ? xhr.responseJSON.message : (err || 'Unknown error');
                showMessage('Bench API call failed: ' + msg);
            }
        })
        .always(() => {
            $('#mainSpinner').hide();
            $('#btnAssess').prop('disabled', false);
        });
    }

    // Fetch employees from backend
    function loadEmployees() {
        $('#mainSpinner').show();

        $.ajax({
            url: EMPLOYEE_API,
            method: 'POST',
            dataType: 'json',
            timeout: 7000
        })
        .done(function(data) {
            if (!Array.isArray(data)) {
                showMessage('Employee API returned unexpected data.', true);
                return;
            }

            employees = data.map(e => {
                // Normalize a bit — ensure fields exist
                return {
                    employeeID: e.employeeID,
                    fullName: e.fullName || ('#' + (e.employeeID || 'n/a')),
                    email: e.email || '',
                    phone: e.phone || '',
                    gender: e.gender || '',
                    dateOfJoining: e.dateOfJoining || '',
                    designation: e.designation || '',
                    experienceInYears: e.experienceInYears != null ? e.experienceInYears : 0,
                    isActive: e.isActive != null ? e.isActive : true,
                    skills: Array.isArray(e.skills) ? e.skills : [],
                    skillIDs: Array.isArray(e.skillIDs) ? e.skillIDs : []
                };
            });

                buildSuggestions();
            $('#suggestions').text(`Loaded ${employees.length} employee(s). Type a name to filter.`);
        })
        .fail(function(xhr, status, err) {
            showMessage('Failed loading employees: ' + (err || status));
            $('#suggestions').text('Unable to load suggestions. Check that GET http://localhost:5080/api/Employee is reachable.');
        })
        .always(function(){
            $('#mainSpinner').hide();
        });
    }

    // Wire UI events
    $(function() {
        // initial load
        $('#mainSpinner').show();
        loadEmployees();

        // Listen to input change and update selection
        // input events for custom suggestions
        const $search = $('#employeeSearch');
        let focusedIndex = -1;

        function showFiltered(term) {
            const $list = $('#suggestionList');
            $list.empty();
            if (!term || term.trim().length < 1) { $list.hide(); focusedIndex = -1; return; }

            const q = term.trim().toLowerCase();
            const matches = employees.filter(e => (e.fullName||'').toLowerCase().includes(q));
            if (!matches.length) { $list.hide(); return; }

            matches.forEach((emp, idx) => {
                // highlight match
                const name = emp.fullName || '';
                const start = name.toLowerCase().indexOf(q);
                let html = escapeHtml(name);
                if (start >= 0) {
                    html = escapeHtml(name.substring(0,start)) + '<strong>' + escapeHtml(name.substring(start, start+q.length)) + '</strong>' + escapeHtml(name.substring(start+q.length));
                }

                const $item = $(`<div class="suggestion-item" data-index="${employees.indexOf(emp)}" role="option">${html}</div>`);
                $list.append($item);
            });

            $list.show();
            focusedIndex = -1;
        }

        $search.on('input', function(){
            const val = $(this).val();
            // update selection preview if exact match
            setSelectedByName(val);
            // show suggestions (only if length >= 2 to avoid noise)
            if ((val||'').trim().length >= 1) showFiltered(val);
        });

        // keyboard navigation for suggestions
        $search.on('keydown', function(e){
            const $list = $('#suggestionList');
            const $items = $list.children('.suggestion-item');
            if (!$items.length || !$list.is(':visible')) return;

            if (e.key === 'ArrowDown') {
                e.preventDefault();
                focusedIndex = Math.min(focusedIndex + 1, $items.length - 1);
                $items.removeClass('focused').eq(focusedIndex).addClass('focused');
            } else if (e.key === 'ArrowUp') {
                e.preventDefault();
                focusedIndex = Math.max(focusedIndex - 1, 0);
                $items.removeClass('focused').eq(focusedIndex).addClass('focused');
            } else if (e.key === 'Enter') {
                if (focusedIndex >= 0) {
                    e.preventDefault();
                    $items.eq(focusedIndex).trigger('click');
                } else {
                    // if Enter and exact selection exists, assess
                    if (selectedEmployee) assessBench();
                }
            } else if (e.key === 'Escape') {
                $('#suggestionList').hide();
            }
        });

        // click on suggestion -> select
        $(document).off('click', '.suggestion-item');
        $(document).on('click', '.suggestion-item', function(e){
            const idx = Number($(this).attr('data-index'));
            const emp = employees[idx];
            if (!emp) return;
            $('#employeeSearch').val(emp.fullName);
            setSelectedByName(emp.fullName);
            $('#suggestionList').hide();
        });

        // hide suggestions when clicking outside
        $(document).on('click', function(ev){
            if (!$(ev.target).closest('#employeeSearch, #suggestionList').length) {
                $('#suggestionList').hide();
            }
        });

        // When the user presses Enter while on the input, and a valid employee is selected, assess
        $('#employeeSearch').on('keypress', function(e){
            if (e.which === 13) { // Enter
                e.preventDefault();
                if (selectedEmployee) assessBench();
            }
        });

        $('#btnAssess').on('click', function(){
            assessBench();
        });
    });

})(jQuery);
