'use strict';

/*
 * Persona Bar module: encrypt / decrypt the connectionStrings and appSettings sections of web.config.
 */
define(['jquery', 'knockout'],
    function ($, ko) {
        var utility, viewModel;

        var requestService = function (type, method, params, callback, failure) {
            utility.sf.moduleRoot = 'personaBar';
            utility.sf.controller = 'ConfigProtection';
            utility.sf[type].call(utility.sf, method, params, callback, failure);
        };

        var failureMessage = function (xhr) {
            var message = utility.resx.ConfigProtection.ERROR_Generic;
            if (xhr && xhr.responseJSON && xhr.responseJSON.Message) {
                message = xhr.responseJSON.Message;
            }
            utility.notifyError(message);
        };

        var toRow = function (section) {
            return {
                Name: section.Name,
                IsProtected: section.IsProtected,
                Provider: section.Provider,
                ItemCount: section.ItemCount,
                Error: section.Error,
                selectedProvider: ko.observable(viewModel.defaultProvider)
            };
        };

        var loadStatus = function () {
            requestService('get', 'GetStatus', {}, function (data) {
                viewModel.loadError('');
                viewModel.providers(data.Providers);
                viewModel.defaultProvider = data.DefaultProvider;
                viewModel.sections($.map(data.Sections, toRow));
            }, function (xhr) {
                viewModel.loadError(utility.resx.ConfigProtection.ERROR_Load);
                failureMessage(xhr);
            });
        };

        var change = function (method, section, provider, confirmText) {
            utility.confirm(
                confirmText.replace('{0}', section),
                utility.resx.ConfigProtection.ConfirmButton,
                utility.resx.ConfigProtection.CancelButton,
                function () {
                    requestService('post', method, { Section: section, Provider: provider }, function () {
                        utility.notify(utility.resx.ConfigProtection.Success);
                        loadStatus();
                    }, failureMessage);
                });
        };

        var initViewModel = function () {
            viewModel = {
                resx: utility.resx.ConfigProtection,
                sections: ko.observableArray([]),
                providers: ko.observableArray([]),
                defaultProvider: '',
                loadError: ko.observable(''),
                encrypt: function (row) {
                    change('Protect', row.Name, row.selectedProvider(), utility.resx.ConfigProtection.EncryptConfirm);
                },
                decrypt: function (row) {
                    change('Unprotect', row.Name, null, utility.resx.ConfigProtection.DecryptConfirm);
                }
            };
        };

        var init = function (wrapper, util, params, callback) {
            utility = util;
            initViewModel();
            ko.applyBindings(viewModel, wrapper[0]);
            loadStatus();

            if (typeof callback === 'function') {
                callback();
            }
        };

        var load = function (params, callback) {
            if (viewModel) {
                loadStatus();
            }
            if (typeof callback === 'function') {
                callback();
            }
        };

        return {
            init: init,
            load: load
        };
    });
