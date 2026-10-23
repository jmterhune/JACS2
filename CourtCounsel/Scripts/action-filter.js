/* Court Counsel Module - Action Type filter for search result lists.
 *
 * Adds a multi-select "Action Type" dropdown above a DataTables result table and filters the rows
 * by the table's "Action Taken" column. Nothing ticked means all action types. The options are the
 * distinct actions found in the results, so the list only offers what can actually match.
 *
 * Usage (after the table's DataTable() call):
 *   CCActionFilter.init({ table: '#case-list', host: '#actionFilterHost' });
 * The table needs a header cell marked with the data-action-col attribute.
 */
(function ($) {
    var NONE = '(No action)';
    var FLAGGED = /vexatious|(enjoin.*pro se|pro se.*enjoin)/i;

    function cellText(settings, rowIdx, col) {
        var cells = settings.aoData[rowIdx].anCells;
        return cells && cells[col] ? $.trim(cells[col].textContent) : '';
    }

    function init(opts) {
        var $table = $(opts.table);
        var $host = $(opts.host);
        if (!$table.length || !$host.length || !$.fn.DataTable || !$.fn.DataTable.isDataTable($table)) return;

        var dt = $table.DataTable();
        var settings = dt.settings()[0];
        var col = $table.find('thead th').index($table.find('thead th[data-action-col]'));
        if (col < 0) return;

        var selected = {};   // action text -> true; empty object = all
        var values = {};
        dt.rows().every(function () {
            values[cellText(settings, this.index(), col) || NONE] = true;
        });
        var names = Object.keys(values).sort(function (a, b) {
            if (a === NONE) return 1;
            if (b === NONE) return -1;
            return a.toLowerCase() < b.toLowerCase() ? -1 : 1;
        });

        var $root = $(
            '<div class="action-dropdown dropdown d-inline-block">' +
              '<button type="button" class="form-select text-start action-toggle" data-bs-toggle="dropdown" data-bs-auto-close="outside" aria-expanded="false" aria-label="Action type">' +
                '<span class="action-label">All Action Types</span>' +
              '</button>' +
              '<div class="dropdown-menu dropdown-menu-end p-2">' +
                '<input type="text" class="form-control form-control-sm mb-2 action-filter" placeholder="Filter action types..." autocomplete="off" />' +
                '<div class="form-check border-bottom pb-1 mb-1">' +
                  '<input type="checkbox" class="form-check-input action-select-all" checked="checked" />' +
                  '<label class="form-check-label fw-bold">All Action Types</label>' +
                '</div>' +
                '<div class="action-checklist"></div>' +
                '<div class="d-flex justify-content-between align-items-center border-top pt-2 mt-1 small text-muted">' +
                  '<span class="action-count">All selected</span>' +
                  '<button type="button" class="btn btn-link btn-sm p-0 action-reset">Reset to All</button>' +
                '</div>' +
              '</div>' +
            '</div>');

        var $list = $root.find('.action-checklist');
        $.each(names, function (i, name) {
            var $row = $('<div class="form-check"></div>');
            var $box = $('<input type="checkbox" class="form-check-input" />').attr('id', 'actionOpt' + i).val(name);
            var $lbl = $('<label class="form-check-label"></label>').attr('for', 'actionOpt' + i).text(name);
            if (FLAGGED.test(name)) $lbl.append('<span class="action-flag">Prohibited filer</span>');
            $row.append($box, $lbl);
            $list.append($row);
        });

        var $label = $root.find('.action-label');
        var $count = $root.find('.action-count');
        var $all = $root.find('.action-select-all');
        function $boxes() { return $list.find('input[type=checkbox]'); }

        function update() {
            selected = {};
            $boxes().filter(':checked').each(function () { selected[this.value] = true; });
            var picked = Object.keys(selected);
            $all.prop('checked', picked.length === 0);
            if (picked.length === 0) {
                $label.text('All Action Types');
                $count.text('All selected');
            } else {
                $label.text(picked.length <= 3 ? picked.join(', ') : picked.length + ' action types selected');
                $count.text(picked.length + ' of ' + names.length + ' selected');
            }
            dt.draw();
        }

        $all.on('click', function () { $boxes().prop('checked', false); this.checked = true; update(); });
        $list.on('change', 'input', update);
        $root.find('.action-reset').on('click', function () { $boxes().prop('checked', false); update(); });
        $root.find('.dropdown-menu').on('click', function (e) { e.stopPropagation(); });
        $root.find('.action-filter').on('input', function () {
            var term = this.value.toLowerCase();
            $list.children().each(function () {
                $(this).toggle($(this).text().toLowerCase().indexOf(term) !== -1);
            });
        });

        // Row filter: runs on every draw for this table only.
        $.fn.dataTable.ext.search.push(function (s, data, rowIdx) {
            if (s !== settings) return true;
            if ($.isEmptyObject(selected)) return true;
            return !!selected[cellText(s, rowIdx, col) || NONE];
        });

        $host.empty().append('<label class="fw-bold me-2 text-nowrap">Action Type:</label>').append($root);
    }

    window.CCActionFilter = { init: init };
})(jQuery);
