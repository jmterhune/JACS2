let judgeControllerInstance = null;
class JudgeController {
    constructor(params = {}) {
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin || false;
        this.adminRole = params.adminRole || 'AdminRole';
        this.judgeRole = params.judgeRole || 'Judge';
        this.portalId = params.portalId || -1;
        this.pageSize = params.pageSize || 25;
        this.sortDirection = params.sortDirection || 'asc';
        this.recordCount = params.recordCount || 0;
        this.sortColumnIndex = params.sortColumnIndex || 2;
        this.currentPage = params.currentPage || 0;
        this.judgeId = -1;
        this.searchTerm = "";
        this.judgeTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        judgeControllerInstance = this;
    }

    init() {
        $("#edit_progress_judge").show();
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}JudgeAPI/DeleteJudge/`;

        const listUrl = `${this.service.baseUrl}JudgeAPI/GetJudges/${this.recordCount}`;
        const detailModalElement = document.getElementById('JudgeDetailModal');
        if (detailModalElement) {
            detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }

        const editModalElement = document.getElementById('JudgeEditModal');
        if (editModalElement) {
            editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }
        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });

        this.populateCourts();
        this.populateJudgeUsers();

        this.judgeTable = $('#tblJudge').DataTable({
            searching: true,
            autoWidth: true,
            stateSave: true,
            ajax: {
                url: listUrl,
                type: "GET",
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                data(data) {
                    data.searchText = data.search?.value || '';
                    delete data.columns;
                },
                error: function (error) {
                    $("#tblJudge_processing").hide();
                    if (error.status === 401) {
                        ShowNotification('Error Retrieving Judges', 'Please make sure you are logged in and try again. Error: ' + error.statusText, 'error');
                    } else {
                        ShowNotification('Error Retrieving Judges', 'The following error occurred attempting to retrieve judge information. Error: ' + error.statusText, 'error');
                    }
                }
            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="View Details" data-toggle="tooltip" data-id="${data}" class="judge-detail btn-command"><i class="fas fa-eye"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="Edit Judge" data-toggle="tooltip" data-id="${data}" class="judge-edit btn-command"><i class="fas fa-pencil"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "name",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "phone",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "court_name",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "title",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "id",
                    render: function (data, type, row) {
                        if (isAdmin === "True") {
                            return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete Judge" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
                        }
                        return '';
                    },
                    className: "command-item",
                    orderable: false
                },
            ],
            language: {
                emptyTable: "No Records Available.",
                zeroRecords: "No records match the search criteria you entered."
            },
            order: [[this.sortColumnIndex, this.sortDirection]],
            serverSide: true,
            processing: true,
            lengthMenu: [[25, 50, 100], [25, 50, 100]],
            pageLength: this.pageSize,
            displayStart: this.currentPage * this.pageSize,
        });

        $(".dt-length").prepend($("#lnkAdd"));
        this.judgeTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const judgeId = $(this).data("id");
                Swal.fire({
                    title: 'Delete Judge?',
                    text: 'Are you sure you wish to delete this Judge?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        judgeControllerInstance.DeleteJudge(judgeId);
                    }
                });
            });
        });

        $(document).on('click', '.judge-detail', function (e) {
            e.preventDefault();
            var judgeId = $(this).data("id");
            judgeControllerInstance.ViewJudge(judgeId, false);
        });

        const editModal = new bootstrap.Modal(document.getElementById('JudgeEditModal'));
        $(document).on('click', '.judge-edit, #editJudgeBtn', function (e) {
            e.preventDefault();
            var judgeId = $(this).data("id") || $("#hdJudgeId").val();
            judgeControllerInstance.judgeId = judgeId;
            if (judgeId) {
                judgeControllerInstance.ViewJudge(judgeId, true);
                $("#JudgeEditModalLabel").html(`Edit Judge`);
                $("#edit_judgeName").hide();
                $("#edit_judgeNameText").show();
            } else {
                judgeControllerInstance.ClearEditForm();
                judgeControllerInstance.populateCourts(false);
                $("#JudgeEditModalLabel").html("Create New Judge");
                $("#edit_judgeName").show();
                $("#edit_judgeNameText").hide();
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            judgeControllerInstance.ClearEditForm();
            judgeControllerInstance.populateCourts(false);
            $("#JudgeEditModalLabel").html("Create New Judge");
            $("#edit_judgeName").show();
            $("#edit_judgeNameText").hide();
            editModal.show();
        });

        $("#edit_judgeName, #edit_judgeNameText").on("change", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });

        $("#cmdDelete").on("click", function (e) {
            e.preventDefault();
            var judgeId = $("#hdJudgeId").val();
            Swal.fire({
                title: 'Delete Judge?',
                text: 'Are you sure you wish to delete this Judge?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    judgeControllerInstance.DeleteJudge(judgeId);
                }
            });
        });

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;

            const $judgeName = $("#edit_judgeName").is(":visible") ? $("#edit_judgeName") : $("#edit_judgeNameText");
            const $judgeNameError = $judgeName.next(".invalid-feedback");
            if ($judgeName.val().trim() === "") {
                $judgeNameError.show();
                $judgeName.addClass("is-invalid");
                isValid = false;
            } else {
                $judgeNameError.hide();
                $judgeName.removeClass("is-invalid");
            }

            if (isValid) {
                judgeControllerInstance.SaveJudge();
            }
        });
    }

    populateJudgeUsers() {
        $.ajax({
            url: `${this.service.baseUrl}JudgeAPI/GetJudgeDropDownItems/${this.judgeRole}/0`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                const $judgeSelect = $("#edit_judgeName");
                $judgeSelect.empty();
                $judgeSelect.append('<option value="">Select Judge</option>');
                if (response.data) {
                    response.data.forEach(user => {
                        $judgeSelect.append(`<option value="${user.Key}">${user.Value}</option>`);
                    });
                } else {
                    ShowNotification("Error", response.error || "Failed to load judge users.", 'error');
                }
            },
            error: function (error) {
                ShowNotification("Error Loading Judge Users", error.statusText || "Failed to load judge users. Please try again later.", 'error');
            }
        });
    }

    populateCourts(isEditMode = false, judgeCourtId = null, judgeCourtName = null) {
        $.ajax({
            url: `${this.service.baseUrl}CourtAPI/GetCourtsUnassigned`,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                const $courtSelect = $("#edit_judgeCourt");
                $courtSelect.empty();
                $courtSelect.append('<option value="">Select Court</option>');

                let courts = response.data || [];

                if (isEditMode && judgeCourtId && judgeCourtName) {
                    courts = [{ id: judgeCourtId, description: judgeCourtName }, ...courts];
                }
                courts.forEach(court => {
                    if (court.id === judgeCourtId) {
                        $courtSelect.append(`<option value="${court.id}" selected>${court.description}</option>`);
                    } else {
                        $courtSelect.append(`<option value="${court.id}">${court.description}</option>`);
                    }
                });

                $("#edit_progress_judge").hide();
            },
            error: function (error) {
                ShowNotification("Error Loading Courts", error.statusText || "Failed to load unassigned courts. Please try again later.", 'error');
                $("#edit_progress_judge").hide();
            }
        });
    }

    ClearState() {
        if (this.judgeTable) {
            this.judgeTable.state.clear();
            window.location.reload();
        }
    }

    DeleteJudge(judgeId) {
        $.ajax({
            url: this.deleteUrl + judgeId,
            type: 'GET',
            dataType: 'json',
            beforeSend: xhr => this.setAjaxHeaders(xhr),
            success: function (response) {
                if (response.status === 200) {
                    if (judgeControllerInstance.judgeTable) {
                        judgeControllerInstance.judgeTable.draw();
                    }
                    const editModal = bootstrap.Modal.getInstance(document.getElementById('JudgeEditModal'));
                    if (editModal) {
                        editModal.hide();
                    }
                    const detailModal = bootstrap.Modal.getInstance(document.getElementById('JudgeDetailModal'));
                    if (detailModal) {
                        detailModal.hide();
                    }
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: response.message || 'Judge deleted successfully.'
                    });
                } else {
                    ShowNotification("Error", response.message || "Unexpected error occurred.", 'error');
                }
            },
            error: function (error) {
                ShowNotification("Error Deleting Judge", error.statusText, 'error');
            }
        });
    }

    ClearEditValidations() {
        $("#edit_judgeName").removeClass("is-invalid");
        $("#edit_judgeNameText").removeClass("is-invalid");
        $("#edit_judgeName").next(".invalid-feedback").hide();
        $("#edit_judgePhone").removeClass("is-invalid");
        $("#edit_judgePhone").next(".invalid-feedback").hide();
        $("#edit_judgeCourt").removeClass("is-invalid");
        $("#edit_judgeCourt").next(".invalid-feedback").hide();
        $("#edit_judgeTitle").removeClass("is-invalid");
        $("#edit_judgeTitle").next(".invalid-feedback").hide();
    }

    ViewJudge(judgeId, isEditMode = false) {
        const getUrl = `${this.service.baseUrl}JudgeAPI/GetJudge/${judgeId}`;
        const progressId = isEditMode ? "#edit_progress_judge" : "#progress-judge";
        $(progressId).show();

        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('JudgeDetailModal'));
            if (!modal._element.classList.contains('show')) {
                modal.show();
            }
        }

        if (judgeId) {
            $.ajax({
                url: getUrl,
                method: 'GET',
                dataType: 'json',
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    if (response.data) {
                        if (isEditMode) {
                            $("#edit_hdJudgeId").val(response.data.id);
                            $("#edit_judgeNameText").val(response.data.name);
                            $("#edit_judgePhone").val(response.data.phone);
                            $("#edit_judgeCourt").val(response.data.court_id || "");
                            $("#edit_judgeTitle").val(response.data.title);
                            $("#JudgeEditModalLabel").html(`Edit Judge: ${response.data.name}`);
                            judgeControllerInstance.populateCourts(true, response.data.court_id, response.data.court_name);
                        } else {
                            $("#judgeName").html(response.data.name);
                            $("#judgePhone").html(response.data.phone);
                            $("#judgeCourt").html(response.data.court_name);
                            $("#judgeTitle").html(response.data.title);
                            $("#hdJudgeId").val(response.data.id);
                        }
                        $(progressId).hide();
                    } else {
                        ShowNotification('Error', response.error || 'Failed to retrieve judge details. Please try again later.', 'error');
                        $(progressId).hide();
                    }
                },
                error: function (error) {
                    ShowNotification('Error Retrieving Judge Details', error.statusText || 'Failed to retrieve judge details. Please try again later.', 'error');
                    $(progressId).hide();
                }
            });
        } else {
            $(progressId).hide();
        }
    }

    SaveJudge() {
        const $judgeName = $("#edit_judgeName").is(":visible") ? $("#edit_judgeName") : $("#edit_judgeNameText");
        if ($judgeName.val().trim() === "") {
            $judgeName.addClass("is-invalid");
            $judgeName.next(".invalid-feedback").show();
            return;
        }

        if ($("#edit_hdJudgeId").val() === "") {
            this.CreateJudge();
        } else {
            this.UpdateJudge();
        }
        if (judgeControllerInstance.judgeTable) {
            judgeControllerInstance.ClearEditForm();
            judgeControllerInstance.judgeTable.draw();
        }
    }

    CreateJudge() {
        try {
            $("#edit_progress_judge").show();
            const judgeData = {
                id: $("#edit_judgeName").val(),
                name: $("#edit_judgeName option:selected").text().trim(),
                phone: $("#edit_judgePhone").val().trim(),
                court_id: $("#edit_judgeCourt").val() || null,
                title: $("#edit_judgeTitle").val().trim()
            };
            $.ajax({
                url: `${this.service.baseUrl}JudgeAPI/CreateJudge`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(judgeData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress_judge").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Judge created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('JudgeEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (judgeControllerInstance.judgeTable) {
                            judgeControllerInstance.judgeTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while creating judge.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress_judge").hide();
                    ShowNotification("Error Creating Judge", error.statusText || "Failed to create judge.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress_judge").hide();
            ShowNotification("Error Creating Judge", e.message, 'error');
        }
    }

    UpdateJudge() {
        try {
            $("#edit_progress_judge").show();
            const judgeData = {
                id: parseInt($("#edit_hdJudgeId").val()),
                name: $("#edit_judgeNameText").val().trim(),
                phone: $("#edit_judgePhone").val().trim(),
                court_id: $("#edit_judgeCourt").val() || null,
                title: $("#edit_judgeTitle").val().trim()
            };
            $.ajax({
                url: `${this.service.baseUrl}JudgeAPI/UpdateJudge`,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify(judgeData),
                beforeSend: xhr => this.setAjaxHeaders(xhr),
                success: function (response) {
                    $("#edit_progress_judge").hide();
                    if (response && response.status === 200) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || 'Judge updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('JudgeEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                        if (judgeControllerInstance.judgeTable) {
                            judgeControllerInstance.judgeTable.draw();
                        }
                    } else {
                        ShowNotification("Error", response.message || "Unexpected error occurred while updating judge.", 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress_judge").hide();
                    ShowNotification("Error Updating Judge", error.statusText || "Failed to update judge.", 'error');
                }
            });
        } catch (e) {
            $("#edit_progress_judge").hide();
            ShowNotification("Error Updating Judge", e.message, 'error');
        }
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'JudgeDetailModal') {
            judgeControllerInstance.ClearDetailForm();
        } else if (modalId === 'JudgeEditModal') {
            judgeControllerInstance.ClearEditForm();
            judgeControllerInstance.ClearEditValidations();
        }
    }

    ClearDetailForm() {
        $("#judgeName").html("");
        $("#judgePhone").html("");
        $("#judgeCourt").html("");
        $("#judgeTitle").html("");
        $("#hdJudgeId").val("");
    }

    ClearEditForm() {
        $("#edit_judgeName").val("");
        $("#edit_judgeNameText").val("");
        $("#edit_judgePhone").val("");
        $("#edit_judgeCourt").val("");
        $("#edit_judgeTitle").val("");
        $("#edit_hdJudgeId").val("");
    }

    setAjaxHeaders(xhr) {
        xhr.setRequestHeader('ModuleId', this.moduleId);
        xhr.setRequestHeader('TabId', this.service.framework.getTabId());
        xhr.setRequestHeader('RequestVerificationToken', this.service.framework.getAntiForgeryValue());
    }
}