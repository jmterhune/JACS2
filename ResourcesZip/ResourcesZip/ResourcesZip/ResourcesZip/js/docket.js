let docketControllerInstance = null;
class DocketController {
    constructor(options) {
        this.moduleId = options.moduleId || -1;
        this.userId = options.userId || -1;
        this.isAdmin = options.isAdmin === "True";
        this.adminRole = options.adminRole || 'Admin';
        this.service = options.service || null;
        docketControllerInstance = this;
    }

    init() {
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        const baseUrl = this.service.baseUrl;
        if (this.isAdmin) {
            this.userId = 0;
        }
        this.initFilters(baseUrl);
        this.bindEvents();
    }

    initFilters(baseUrl) {
        const initializeSelect2 = (selector, url, placeholder, hasAjax = true) => {
            if (hasAjax) {
                $(selector).select2({
                    ajax: {
                        url: url,
                        dataType: 'json',
                        delay: 500,
                        data: params => ({
                            q: params.term,
                            page: params.page
                        }),
                        beforeSend: xhr => this.setAjaxHeaders(xhr),
                        processResults: response => {
                            if (response.data) {
                                return {
                                    results: response.data.map(item => ({
                                        id: item.Key,
                                        text: item.Value
                                    }))
                                };
                            }
                            return { results: [] };
                        },
                        error: () => {
                            ShowNotification("Error", `Failed to retrieve records for ${selector}. Please try again later.`, 'error');
                        },
                        cache: false
                    },
                    placeholder: placeholder,
                    allowClear: true,
                    theme: "bootstrap-5",
                    minimumInputLength: 0,
                    width: '100%'
                });
            } else {
                $(selector).select2({
                    placeholder: placeholder,
                    allowClear: true,
                    theme: "bootstrap-5",
                    minimumInputLength: 0,
                    width: '100%'
                });
            }
        };

        initializeSelect2("#courtFilter", `${baseUrl}JudgeAPI/GetJudgeCourtDropDownItems/${this.userId}`, "Select Judge");
        $.ajax({
            url: `${baseUrl}JudgeAPI/GetJudgeCourtDropDownItems/${this.userId}`,
            type: "GET",
            data: { q: '' },
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: (response) => {
                if (response.data) {
                    response.data.forEach(item => {
                        $('#courtFilter').append(`<option value="${item.Key}">${item.Value}</option>`);
                    });
                }
            },
            error: () => {
                ShowNotification("Error", "Failed to load judges.", 'error');
            }
        });

        initializeSelect2("#categoryFilter", null, "All", false); // No AJAX for categories, but load below

        // Load all categories statically (mimicking PHP foreach)
        $.ajax({
            url: `${baseUrl}CategoryAPI/GetCategoryDropDownItems/`,
            type: "GET",
            data: { q: '' },
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: (response) => {
                if (response.data) {
                    response.data.forEach(item => {
                        $('#categoryFilter').append(`<option value="${item.Key}">${item.Value}</option>`);
                    });
                }
            },
            error: () => {
                ShowNotification("Error", "Failed to load categories.", 'error');
            }
        });

        $('.categories_select').select2();
    }

    bindEvents() {
        $(document).on('select2:open', () => {
            document.querySelector('.select2-search__field').focus();
        });

        $('#courtFilter').on('change', () => {
            this.getCategory();
        });

        $('#printDocket').on('click', () => {
            const baseUrl = this.service.baseUrl;
            const postData = {
                court: $("#courtFilter").val() || 0,
                category: $("#categoryFilter").val() || 0,
                from: $("#from").val(),
                to: $("#to").val(),
                hearing: $('input[name="hearing"]:checked').val() || 'all',
                category_print: $("#categoryPrint").is(":checked")
            };

            if (!postData.court) {
                ShowNotification("Error", "Please select a judge.", 'error');
                return;
            }
            if (!postData.from) {
                ShowNotification("Error", "Please select a from date.", 'error');
                return;
            }

            $.ajax({
                url: `${baseUrl}DocketAPI/GenerateDocketReport/`,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(postData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                xhrFields: {
                    responseType: 'blob'
                },
                success: (blob) => {
                    const url = window.URL.createObjectURL(blob);
                    const a = document.createElement('a');
                    a.href = url;
                    a.download = 'docket.docx';
                    document.body.appendChild(a);
                    a.click();
                    a.remove();
                },
                error: (xhr) => {
                    ShowNotification("Error", "Failed to generate docket report.", 'error');
                }
            });
        });

        // Initial call if a court is pre-selected
        this.getCategory();
    }

    getCategory() {
        const courtId = $('#courtFilter').val();
        if (!courtId) return;

        const baseUrl = this.service.baseUrl;
        $.ajax({
            url: `${baseUrl}CourtAPI/GetCourt/${courtId}`,
            method: 'GET',
            dataType: 'JSON',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                let checkValue = (response.data && response.data.category_print) ? true : false;
                $('#categoryPrint').prop('checked', checkValue);
            },
            error: function () {
                ShowNotification("Error", "Failed to load court details.", 'error');
            }
        });
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}