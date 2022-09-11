(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as an anonymous module.
        define(['angular', 'jquery'], factory);
    } else if (typeof exports === 'object') {
        // Node. Does not work with strict CommonJS, but
        // only CommonJS-like environments that support module.exports, like Node.
        module.exports = factory(require('angular'), require('jquery'));
    } else {
        // Browser globals (root is window)
        root.returnExports = factory(root.angular, root.jQuery);
    }
}(this, function (angular, $) {
    angular.module('ng-trunk8', []).provider('trunk8Config', function () {
        var config = {}

        this.$get = function () {
            return config
        }

        this.setConfig = function (cfg) {
            config = cfg
        }
    }).directive('trunk8', ['trunk8Config', '$window', function (trunk8Config,  $window) {

        return {
            restrict: 'EA',
            scope: {
                trunk8: '='
            },
            link: function (scope, elem, attrs) {
                var config = angular.extend({
                    expendable: false,
                    resizable: true,
                    more: 'more',
                    less: 'less',
                    fill: '&hellip;'
                }, trunk8Config, scope.trunk8)

                function getRandomId() {
                    return Date.now() + '-' + Math.floor(Math.random() * (10000000 - 1)) + 1;
                }

                var readMoreId = 'more-' + getRandomId()
                var moreBtn = '<a href="javascript:;" id="' + readMoreId + '">' + config.more + '</a>'
                var lessBtn = $('<a href="javascript:;">' + config.less + '</a>')

                if (config.expendable) {
                    config.fill += ' ' + moreBtn
                }
                elem.trunk8(config)

                // for expend/collapse
                if (config.expendable) {
                    elem.on('click', '#' + readMoreId, function (evt) {
                        elem.trunk8('revert').append(lessBtn)
                        lessBtn.on('click', function (evt) {
                            elem.trunk8()
                        })
                    })
                }

                // watch text
                if (config.text) {
                    scope.$watch('trunk8.text', function (nValue) {
                        elem.trunk8('update', nValue)
                    })
                }

                // for resize
                if (config.resizable) {
                    $($window).resize(function () {
                        elem.trunk8()
                    })
                }
            }
        }
    }])
}));