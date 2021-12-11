/// <binding AfterBuild="prod" />
/*
This file in the main entry point for defining grunt tasks and using grunt plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkID=513275&clcid=0x409
*/
console.log(`Node ${process.version}`); // node version
module.exports = function (grunt) {
    'use strict';
    var sass = require('node-sass'); // used in grunt-sass options, must be included in package.json
    
    grunt.loadNpmTasks("grunt-sass");
    grunt.loadNpmTasks("grunt-postcss");
    grunt.loadNpmTasks("grunt-contrib-watch");
    grunt.loadNpmTasks("grunt-contrib-concat");
    grunt.loadNpmTasks("grunt-contrib-copy");
    grunt.loadNpmTasks("grunt-contrib-cssmin");

    grunt.initConfig({
        pkg: grunt.file.readJSON("package.json"),

        // Sass
        sass: {
            options: {
                implementation: sass,
                sourceMap: false, // Create source map
                outputStyle: "nested" // Minify output with "compressed"
            },
            dist: {
                files: [
                    {
                        "wwwroot/lib/bootstrap/bootstrap.css": "Styles/bootstrap/bootstrap-custom.scss",  // "destination": "source"
                        "wwwroot/css/site.css": "Styles/site/site.scss"
                    }
                ]
            }
        },

        // Postcss
        postcss: {
            options: {
                // map: true, // inline sourcemaps
                // or
                map: {
                    inline: false, // save all sourcemaps as separate files...
                    annotation: "wwwroot/css/maps/", // ...to the specified directory relative to the project root
                    sourcesContent: true // whether original contents (e.g. Sass sources) will be included to a sourcemap. 
                },

                processors: [
                    require("pixrem")(), // add fallbacks for rem units
                    require("autoprefixer")(), // add vendor prefixes
                    require("cssnano")() // minify the result
                ]
            },
            dist: {  // the dist object will hold information on where our CSS files should be read from and written to.
                // src: "css/*.css"
                //src: "css/bootstrap.css"
                //dest: "css/bootstrap.min.css" // if dest: is missing, src will be replaced
                files: {
                    "wwwroot/lib/bootstrap/bootstrap.min.css": "wwwroot/lib/bootstrap/bootstrap.css",
                    "wwwroot/css/site.min.css": "wwwroot/css/site.css"
                }
            }
        },
        concat: {
            jquery: {
                src: ["node_modules/jquery/dist/jquery.min.js"],
                dest: "wwwroot/lib/jquery/jquery.min.js",
                nonull: true
            },
            jquery_validation: {
                src: [
                    "node_modules/jquery-validation/dist/jquery.validate.min.js",
                    "node_modules/jquery-validation/dist/localization/methods_de.js",
                    "node_modules/jquery-validation/dist/localization/messages_de.js",
                    "node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js"
                ],
                dest: "wwwroot/lib/jquery-validation/jquery-validation-all.min.js",
                nonull: true
            },
            bootstrap_js_all: {
                src: ["node_modules/popper.js/dist/umd/popper.min.js",
                    "node_modules/popper.js/dist/umd/popper-utils.min.js",
                    "node_modules/tether/dist/js/tether.min.js",
                    "node_modules/bootstrap/dist/js/bootstrap.min.js",
                    "node_modules/bootstrap-select/dist/js/bootstrap-select.min.js"
                    ],
                dest: "wwwroot/lib/bootstrap/bootstrap-all.min.js",
                nonull: true
            },
            bootstrap_css_all: {
                src: ["wwwroot/lib/bootstrap/bootstrap.min.css",
                    "node_modules/bootstrap-select/dist/css/bootstrap-select.min.css"
                ],
                dest: "wwwroot/lib/bootstrap/bootstrap.min.css",
                nonull: true
            },
            moment_js: {
                src: ["ScriptLib/Moment/moment.min.js"],
                dest: "wwwroot/lib/Moment/moment.min.js",
                nonull: true
            },
            datatables_js: {
                src: ["ScriptLib/DataTables/datatables.min.js",
                    "ScriptLib/DataTables/datetime-moment.min.js"],
                dest: "wwwroot/lib/DataTables/datatables-for-moment.min.js"
            },
            datatables_css: {
                src: ["ScriptLib/DataTables/datatables.min.css",
                    "ScriptLib/DataTables/datetime-moment.min.css"],
                dest: "wwwroot/lib/DataTables/datatables-for-moment.min.css"
            },
            datatables_txt: {
                src: ["ScriptLib/DataTables/dataTables.german.lang.json"],
                dest: "wwwroot/lib/DataTables/dataTables.german.lang.json",
                nonull: true
            },
            jquery_ui_date_time_picker_js: {
                src: ["ScriptLib/DateTimePicker/jquery-ui.min.js",
                    "ScriptLib/DateTimePicker/datepicker-de.js",
                    "ScriptLib/DateTimePicker/jquery.ui.timepicker.min.js"],
                dest: "wwwroot/lib/DateTimePicker/jquery-ui-date-time-picker.min.js",
                nonull: true
            },
            jquery_ui_date_time_picker_css: {
                src: ["ScriptLib/DateTimePicker/jquery-ui.min.css",
                    "ScriptLib/DateTimePicker/jquery-ui.structure.min.css",
                    "ScriptLib/DateTimePicker/jquery-ui.theme.min.css",
                    "ScriptLib/DateTimePicker/jquery.ui.timepicker.css",
                    "ScriptLib/DateTimePicker/tournament-modifications.css"],
                dest: "wwwroot/lib/DateTimePicker/jquery-ui-date-time-picker.min.css",
                nonull: true
            }
        },

        copy: {
            jquery_ui_date_time_picker_img: {
                files: [
                    { cwd:"ScriptLib/DateTimePicker/Images/", expand:"true", src:"**/*", dest: "wwwroot/lib/DateTimePicker/Images/" }
                ]
            }
        },

        // Watch
        watch: {
            css: {
                files: ["Styles/**/*.scss"],
                tasks: ["sass", "postcss"],
                options: {
                    spawn: false
                }
            }
        }
    });

    grunt.registerTask("dev", ["watch"]);
    grunt.registerTask("prod", ["sass", "postcss", "concat", "copy"]);
};