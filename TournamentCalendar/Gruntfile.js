/// <binding AfterBuild='prod' />
/*
This file in the main entry point for defining grunt tasks and using grunt plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkID=513275&clcid=0x409
*/
console.log(`Node ${process.version}`); // node version
module.exports = function (grunt) {
    'use strict';
    var sass = require('node-sass'); // used in grunt-sass options, must be included in package.json
    
    grunt.loadNpmTasks('grunt-sass');
    grunt.loadNpmTasks('grunt-postcss');
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-contrib-cssmin');
    grunt.loadNpmTasks('grunt-contrib-uglify');

    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),

        // Sass
        sass: {
            options: {
                implementation: sass,
                sourceMap: false, // Create source map
                outputStyle: 'nested' // Minify output with 'compressed'
            },
            dist: {
                files: [
                    {
                        'wwwroot/lib/bootstrap/bootstrap.css': 'Styles/bootstrap/bootstrap-custom.scss',  // 'destination': 'source'
                        'wwwroot/css/site.css': 'Styles/site/site.scss'
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
                    annotation: 'wwwroot/css/maps/', // ...to the specified directory relative to the project root
                    sourcesContent: true // whether original contents (e.g. Sass sources) will be included to a sourcemap. 
                },

                processors: [
                    require('pixrem')(), // add fallbacks for rem units
                    require('autoprefixer')(), // add vendor prefixes
                    require('cssnano')() // minify the result
                ]
            },
            dist: {  // the dist object will hold information on where our CSS files should be read from and written to.
                // src: 'css/*.css'
                //src: 'css/bootstrap.css'
                //dest: 'css/bootstrap.min.css' // if dest: is missing, src will be replaced
                files: {
                    'wwwroot/lib/bootstrap/bootstrap.min.css': 'wwwroot/lib/bootstrap/bootstrap.css',
                    'wwwroot/css/site.min.css': 'wwwroot/css/site.css'
                }
            }
        },
        uglify: {
            Location: {
                files: {
                    'Scripts/Site.Location.min.js': ['Scripts/Site.Location.js']
                }
            },
            TempusDominusFactory: {
                files: {
                    'Scripts/Site.TempusDominusFactory.min.js': ['Scripts/Site.TempusDominusFactory.js']
                }
            }
        },
        concat: {
            js_nlog: {
                src: ['node_modules/jsnlog/jsnlog.min.js'],
                dest: 'wwwroot/lib/jsnlog/jsnlog.min.js',
                nonull: true
            },
            bootstrap_js_all: {
                src: ['node_modules/@popperjs/core/dist/umd/popper.min.js',
                      'node_modules/bootstrap/dist/js/bootstrap.min.js'],
                dest: 'wwwroot/lib/bootstrap/bootstrap-all.min.js',
                nonull: true
            },
            bootstrap_css_all: {
                src: ['wwwroot/lib/bootstrap/bootstrap.min.css',
                    'node_modules/bootstrap-icons/font/bootstrap-icons.css'
                ],
                dest: 'wwwroot/lib/bootstrap/bootstrap.min.css',
                nonull: true
            },
            simple_datatables_js: {
                src: ['node_modules/simple-datatables/dist/umd/simple-datatables.js'],
                dest: 'wwwroot/lib/simple-datatables/simple-datatables.min.js',
                nonull: true
            },
            simple_datatables_css: {
                src: ['node_modules/simple-datatables/dist/css/style.css'],
                dest: 'wwwroot/lib/simple-datatables/simple-datatables.css',
                nonull: true
            },
            tempus_dominus_all_js: {
                src: ['node_modules/@eonasdan/tempus-dominus/dist/js/tempus-dominus.min.js',
                'node_modules/@eonasdan/tempus-dominus/dist/locales/de.js',
                'node_modules/@eonasdan/tempus-dominus/dist/plugins/bi-one.js',
                'Scripts/Site.TempusDominusFactory.min.js'],
                dest: 'wwwroot/lib/tempus-dominus/tempus-dominus.all.min.js',
                nonull: true
            },
            tempus_dominus_css: {
                src: ['node_modules/@eonasdan/tempus-dominus/dist/css/tempus-dominus.min.css'],
                dest: 'wwwroot/lib/tempus-dominus/tempus-dominus.min.css',
                nonull: true
            }
        },
        copy: {
            bootstrap_icons_fonts: {
                files: [
                    { cwd: 'node_modules/bootstrap-icons/font/fonts/', expand: 'true', src: '**/*', dest: 'wwwroot/lib/bootstrap/fonts/' }
                ]
            },
            location: {
                src: ['Scripts/Site.Location.min.js'],
                dest: 'wwwroot/js/Site.Location.min.js',
                nonull: true
            }
        },

        // Watch
        watch: {
            css: {
                files: ['Styles/**/*.scss'],
                tasks: ['sass', 'postcss'],
                options: {
                    spawn: false
                }
            }
        }
    });

    grunt.registerTask('dev', ['watch']);
    grunt.registerTask('prod', ['sass', 'postcss', 'uglify', 'concat', 'copy']);
};