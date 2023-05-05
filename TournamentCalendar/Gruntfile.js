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
    grunt.loadNpmTasks("grunt-contrib-uglify");

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
        uglify: {
            Location: {
                files: {
                    'ScriptLib/TournamentCalendar/Location.min.js': ['ScriptLib/TournamentCalendar/Location.js']
                }
            }
        },
        concat: {
            js_nlog: {
                src: ['node_modules/jsnlog/jsnlog.min.js'],
                dest: 'wwwroot/lib/jsnlog/jsnlog.min.js',
                nonull: true
            },
            jquery: {
                src: ["node_modules/jquery/dist/jquery.min.js"],
                dest: "wwwroot/lib/jquery/jquery.min.js",
                nonull: true
            },
            bootstrap_js_all: {
                src: ["node_modules/bootstrap/dist/js/bootstrap.bundle.min.js"],
                dest: "wwwroot/lib/bootstrap/bootstrap-all.min.js",
                nonull: true
            },
            bootstrap_css_all: {
                src: ["wwwroot/lib/bootstrap/bootstrap.min.css",
                    "node_modules/bootstrap-icons/font/bootstrap-icons.css"
                ],
                dest: "wwwroot/lib/bootstrap/bootstrap.min.css",
                nonull: true
            },
            moment_js: {
                src: ["ScriptLib/Moment/moment.min.js"],
                dest: "wwwroot/lib/Moment/moment.min.js",
                nonull: true
            },
            flatpickr_js: {
                src: ["node_modules/flatpickr/dist/flatpickr.min.js", 
                    "node_modules/flatpickr/dist/l10n/default.js", 
                    "node_modules/flatpickr/dist/l10n/de.js"],
                dest: "wwwroot/lib/flatpickr/flatpickr_all.min.js",
                nonull: true
            },
            flatpickr_css: {
                src: ["Styles/flatpickr/flatpickr_custom.css"],
                dest: "wwwroot/lib/flatpickr/flatpickr.css",
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
            }
        },
        copy: {
            bootstrap_icons_fonts: {
                files: [
                    { cwd: 'node_modules/bootstrap-icons/font/fonts/', expand: 'true', src: '**/*', dest: 'wwwroot/lib/bootstrap/fonts/' }
                ]
            },
            jquery_ui_date_time_picker_img: {
                files: [
                    { cwd:"ScriptLib/DateTimePicker/Images/", expand:"true", src:"**/*", dest: "wwwroot/lib/DateTimePicker/Images/" }
                ]
            },
            location: {
                src: ["ScriptLib/TournamentCalendar/Location.min.js"],
                dest: "wwwroot/js/Location.min.js",
                nonull: true
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