module.exports = function (grunt) {

    require('time-grunt')(grunt);
    grunt.initConfig({
		/* Load the package.json so we can use pkg variables */
		pkg: grunt.file.readJSON('package.json'),

        less: {
            development: {
                options: {
                    paths: ["/"],
                    compress:false,
                    ieCompat:true,
                    banner: '/* <%= pkg.name %> - v<%= pkg.version %>  */\n/* STYLES COMPILED FROM SOURCE (LESS) DO NOT MODIFY */\n'
                },
                files: {
                  'css/admin.css': 'less/admin.less',
                  'css/home.css': 'less/home.less',
                  'css/home2.css': 'less/home2.less'
                }
            },
        },

        autoprefixer: {
            development: {
                browsers: [
                    'Android 2.3',
                    'Android >= 4',
                    'Chrome >= 20',
                    'Firefox >= 24', // Firefox 24 is the latest ESR
                    'Explorer >= 8',
                    'iOS >= 6',
                    'Opera >= 12',
                    'Safari >= 6'
                ],
                expand: true,
                flatten: true,
                src: 'css/style.css',
                dest: 'css'
            }
        },

        browserSync: {
            dev: {
                bsFiles: {
                    src: [
                        'css/*.css', 'js/*.js', '**/*.html'
                    ]
                },
                options: {
                    watchTask: true,
                    startPath: "admin.html",
                    server: {
                        baseDir: "./"
                    }
                }
            }
        },

        watch: {
            /* watch for less changes */
            less: {
                files: [
                  'less/**/*.less'
                ],
                tasks: ['less:development']
            },

            /* Reload gruntfile if it changes */
            grunt: {
                files: ['Gruntfile.js']
            }

            /* Add new module here. Mind the comma's :) */
        }
    });

    grunt.loadNpmTasks('grunt-contrib-less');
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks('grunt-autoprefixer');
    grunt.loadNpmTasks('grunt-browser-sync');

    // Default Task For Development
    grunt.registerTask('default', ['less:development', 'browserSync', 'watch']);

};
