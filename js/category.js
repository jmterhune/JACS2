let categoryControllerInstance = null;

class CategoryController {
    constructor(params = {}) {
        this.moduleId = params.moduleId || -1;
        this.userId = params.userId || -1;
        this.isAdmin = params.isAdmin || false;
        this.adminRole = params.adminRole || 'Admin';
        this.pageSize = params.pageSize || 25;
        this.sortDirection = params.sortDirection || 'asc';
        this.recordCount = params.recordCount || 0;
        this.sortColumnIndex = params.sortColumnIndex || 2;
        this.currentPage = params.currentPage || 0;
        this.categoryId = -1;
        this.searchTerm = "";
        this.categoryTable = null;
        this.service = params.service || null;
        this.deleteUrl = null;
        categoryControllerInstance = this;
    }

    init() {
        const isAdmin = this.isAdmin;
        this.service.baseUrl = this.service.framework.getServiceRoot(this.service.path);
        this.deleteUrl = `${this.service.baseUrl}CategoryAPI/DeleteCategory/`;

        const listUrl = `${this.service.baseUrl}CategoryAPI/GetCategories/${this.recordCount}`;
        const detailModalElement = document.getElementById('CategoryDetailModal');
        if (detailModalElement) {
            detailModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }

        const editModalElement = document.getElementById('CategoryEditModal');
        if (editModalElement) {
            editModalElement.addEventListener('hidden.bs.modal', this.onModalClose);
        }
        $(editModalElement).on('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey && !e.ctrlKey && !e.altKey) {
                e.preventDefault();
                $("#edit_cmdSave").trigger('click');
            }
        });
        this.categoryTable = $('#tblCategory').DataTable({
            searching: true,
            autoWidth: true,
            stateSave: true,
            ajax: {
                url: listUrl,
                type: "GET",
                datatype: 'json',
                data(data) {
                    data.searchText = data.search.value;
                    delete data.columns;
                },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                error: function (error) {
                    $("#tblCategory_processing").hide();
                    if (error.status === 401) {
                        ShowNotification("Error Retrieving Categories", "Please make sure you are logged in and try again. Error: " + error.statusText, 'error');
                    } else {
                        ShowNotification("Error Retrieving Categories", "The following error occurred attempting to retrieve category information. Error: " + error.statusText, 'error');
                    }
                }
            },
            columns: [
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="View Details" data-toggle="tooltip" data-id="${data}" class="cat-detail btn-command"><i class="fas fa-eye"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "id",
                    render: function (data) {
                        return `<button type="button" title="Edit Category" data-toggle="tooltip" data-id="${data}" class="cat-edit btn-command"><i class="fas fa-pencil"></i></button>`;
                    },
                    className: "command-item",
                    orderable: false
                },
                {
                    data: "description",
                    render: function (data) {
                        return data || '';
                    }
                },
                {
                    data: "id",
                    render: function (data, type, row) {
                        if (isAdmin === "True") {
                            return `<button type="button" class="delete btn-command" data-toggle="tooltip" aria-role="button" title="Delete Category" data-id="${row.id}"><i class="fas fa-trash"></i></button>`;
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
        this.categoryTable.on('draw', function () {
            $(".delete").on("click", function (e) {
                e.preventDefault();
                const categoryId = $(this).data("id");
                Swal.fire({
                    title: 'Delete Category?',
                    text: 'Are you sure you wish to delete this Category?',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No'
                }).then((result) => {
                    if (result.isConfirmed) {
                        categoryControllerInstance.DeleteCategory(categoryId);
                    }
                });
            });
        });
        $(document).on('click', '.cat-detail', function (e) {
            e.preventDefault();
            var categoryId = $(this).data("id");
            categoryControllerInstance.ViewCategory(categoryId, false);
        });

        const editModal = new bootstrap.Modal(document.getElementById('CategoryEditModal'));
        $(document).on('click', '.cat-edit, #editCategoryBtn', function (e) {
            e.preventDefault();
            var categoryId = $(this).data("id") || $("#hdCategoryId").val();
            categoryControllerInstance.categoryId = categoryId;
            if (categoryId) {
                categoryControllerInstance.ViewCategory(categoryId, true);
                $("#CategoryEditModalLabel").html(`Edit Category`);
            } else {
                categoryControllerInstance.ClearEditForm();
                $("#CategoryEditModalLabel").html("Create New Category");
            }
            editModal.show();
        });

        $("#lnkAdd").on('click', function (e) {
            e.preventDefault();
            categoryControllerInstance.ClearEditForm();
            $("#CategoryEditModalLabel").html("Create New Category");
            editModal.show();
        });

        $("#cmdDelete").on("click", function (e) {
            e.preventDefault();
            var categoryId = $("#hdCategoryId").val();
            Swal.fire({
                title: 'Delete Category?',
                text: 'Are you sure you wish to delete this Category?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    categoryControllerInstance.DeleteCategory(categoryId);
                }
            });
        });

        $("#edit_cmdSave").on("click", function (e) {
            e.preventDefault();
            let isValid = true;

            const $catDescription = $("#edit_catDescription");
            const $catDescriptionError = $catDescription.next(".invalid-feedback");
            if ($catDescription.val().trim() === "") {
                $catDescriptionError.show();
                $catDescription.addClass("is-invalid");
                isValid = false;
            } else {
                $catDescriptionError.hide();
                $catDescription.removeClass("is-invalid");
            }

            if (isValid) {
                categoryControllerInstance.SaveCategory();
            }
        });

        $("#edit_catDescription").on("input", function () {
            const $this = $(this);
            if ($this.val().trim() !== "") {
                $this.next(".invalid-feedback").hide();
                $this.removeClass("is-invalid");
            }
        });
    }

    ClearState() {
        if (this.categoryTable) {
            this.categoryTable.state.clear();
            window.location.reload();
        }
    }

    DeleteCategory(categoryId) {
        $.ajax({
            url: this.deleteUrl + categoryId,
            type: 'GET',
            beforeSend: function (xhr) {
                xhr.setRequestHeader('ModuleId', moduleId);
                xhr.setRequestHeader('TabId', service.framework.getTabId());
                xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
            },
            success: function (result) {
                if (categoryControllerInstance.categoryTable) {
                    categoryControllerInstance.categoryTable.draw();
                }
                const editModal = bootstrap.Modal.getInstance(document.getElementById('CategoryEditModal'));
                if (editModal) {
                    editModal.hide();
                }
                const detailModal = bootstrap.Modal.getInstance(document.getElementById('CategoryDetailModal'));
                if (detailModal) {
                    detailModal.hide();
                }
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Category deleted successfully.'
                });
            },
            error: function (error) {
                ShowNotification("Error Deleting Category", error.statusText, 'error');
            }
        });
    }

    onModalClose(event) {
        const modalId = event.target.id;
        if (modalId === 'CategoryDetailModal') {
            categoryControllerInstance.ClearDetailForm();
        } else if (modalId === 'CategoryEditModal') {
            categoryControllerInstance.ClearEditForm();
            categoryControllerInstance.ClearEditValidations();
        }
    }

    ClearDetailForm() {
        $("#catDescription").html("");
        $("#hdCategoryId").val("");
    }

    ClearEditForm() {
        $("#edit_catDescription").val("");
        $("#edit_hdCategoryId").val("");
    }

    ClearEditValidations() {
        $("#edit_catDescription").removeClass("is-invalid");
        $("#edit_catDescription").next(".invalid-feedback").hide();
    }

    ViewCategory(categoryId, isEditMode = false) {
        const getUrl = `${this.service.baseUrl}CategoryAPI/GetCategory/${categoryId}`;
        const progressId = isEditMode ? "#edit_progress-category" : "#progress-category";
        $(progressId).show();

        if (!isEditMode) {
            const modal = new bootstrap.Modal(document.getElementById('CategoryDetailModal'));
            if (!modal._element.classList.contains('show')) {
                modal.show();
            }
        }

        if (categoryId) {
            $.ajax({
                url: getUrl,
                method: 'GET',
                dataType: 'json',
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                success: function (response) {
                    if (response.data) {
                        if (isEditMode) {
                            $("#edit_hdCategoryId").val(response.data.id);
                            $("#edit_catDescription").val(response.data.description);
                            $("#CategoryEditModalLabel").html(`Edit Category: ${response.data.description}`);
                        } else {
                            $("#catDescription").html(response.data.description);
                            $("#hdCategoryId").val(response.data.id);
                        }
                        $(progressId).hide();
                    } else {
                        ShowNotification("Error", "Failed to retrieve category details. Please try again later.", 'error');
                        $(progressId).hide();
                    }
                },
                error: function () {
                    console.error('Failed to fetch category details');
                    ShowNotification("Error", "Failed to retrieve category details. Please try again later.", 'error');
                    $(progressId).hide();
                }
            });
        } else {
            $(progressId).hide();
        }
    }

    SaveCategory() {
        if ($("#edit_hdCategoryId").val() === "") {
            this.CreateCategory();
        } else {
            this.UpdateCategory();
        }
        if (categoryControllerInstance.categoryTable) {
            categoryControllerInstance.ClearEditForm();
            categoryControllerInstance.categoryTable.draw();
        }
    }

    CreateCategory() {
        try {
            $("#edit_progress-category").show();
            const categoryData = {
                description: $("#edit_catDescription").val()
            };
            $.ajax({
                url: `${this.service.baseUrl}CategoryAPI/CreateCategory`,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(categoryData),
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                success: function (result) {
                    if (result === 200) {
                        $("#edit_progress-category").hide();
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'Category created successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('CategoryEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                    } else {
                        $("#edit_progress-category").hide();
                        ShowNotification("Error", "Unexpected Error: Status=" + result, 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-category").hide();
                    ShowNotification("Error Creating Category", error.statusText, 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-category").hide();
            ShowNotification("Error Creating Category", e.statusText, 'error');
        }
    }

    UpdateCategory() {
        try {
            $("#edit_progress-category").show();
            const categoryData = {
                id: $("#edit_hdCategoryId").val(),
                description: $("#edit_catDescription").val()
            };
            $.ajax({
                url: `${this.service.baseUrl}CategoryAPI/UpdateCategory`,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(categoryData),
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('ModuleId', moduleId);
                    xhr.setRequestHeader('TabId', service.framework.getTabId());
                    xhr.setRequestHeader('RequestVerificationToken', service.framework.getAntiForgeryValue());
                },
                success: function (result) {
                    if (result === 200) {
                        $("#edit_progress-category").hide();
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'Category updated successfully.'
                        });
                        const editModal = bootstrap.Modal.getInstance(document.getElementById('CategoryEditModal'));
                        if (editModal) {
                            editModal.hide();
                        }
                    } else {
                        $("#edit_progress-category").hide();
                        ShowNotification("Error", "Unexpected Error: Status=" + result, 'error');
                    }
                },
                error: function (error) {
                    $("#edit_progress-category").hide();
                    ShowNotification("Error Updating Category", error.statusText, 'error');
                }
            });
        } catch (e) {
            $("#edit_progress-category").hide();
            ShowNotification("Error Updating Category", e.statusText, 'error');
        }
    }
}