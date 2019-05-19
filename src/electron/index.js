/**
 * LiveMe Pro Tools
 */

const appName = 'LiveMe Pro Tools'

const { app, BrowserWindow, ipcMain, Menu, shell } = require('electron')
app.commandLine.appendSwitch('--autoplay-policy','no-user-gesture-required')
const { exec } = require('child_process')
const path = require('path')
let appSettings = require('electron-settings')
const coregateway = require("./coreGateway")

const PORT = 5788;
const HOST = "http://localhost:" + PORT


let mainWindow = null
let playerWindow = null
let bookmarksWindow = null
let chatWindow = null
let wizardWindow = null
let menu = null

coregateway.LogEmitter.on("coreStopped", (x) => {
    mainWindow.loadURL(`file://${__dirname}/loading.html`)
})

coregateway.LogEmitter.on("coreNowRunning", (x) => {
    mainWindow.loadURL(`${HOST}/main/home`)
})





function defaultWindow(w, h) {
    let win = new BrowserWindow({
        icon: path.join(__dirname, 'appicon.ico'),
        width: w,
        height: h,
        autoHideMenuBar: true,
        disableAutoHideCursor: true,
        fullscreen: false,
        maximizable: false,
        frame: false,
        show: false,
        darkTheme: true,
        titleBarStyle: 'default',
        resizable: true,
        backgroundColor: '#999999',
        webPreferences: {
            webSecurity: false,
            textAreasAreResizable: false,
            plugins: true
        }
    })
    win.setMenu(Menu.buildFromTemplate(getMiniMenuTemplate()))
    return win;

}

function createMainOrWizardWindow() {
    let isFreshInstall = appSettings.get('general.fresh_install') == null

    if (isFreshInstall === true) {
        appSettings.set('general', {
            fresh_install: true,
            playerpath: '',
            hide_zeroreplay_fans: false,
            hide_zeroreplay_followings: true,
            enableHomeScan: true
        })
        appSettings.set('position', {
            mainWindow: [-1, -1],
            playerWindow: [-1, -1],
            bookmarksWindow: [-1, -1],
            fansWindow: [-1, -1],
            followingsWindow: [-1, -1]
        })
        appSettings.set('size', {
            mainWindow: [1024, 600],
            playerWindow: [370, 680],
            bookmarksWindow: [400, 720]
        })
        appSettings.set('downloads', {
            path: path.join(app.getPath('home'), 'Downloads'),
            template: '%%replayid%%',
            chunkthreads: 1,
            chunks: 1,
            ffmpegquality: 1
        })
    }

    if (!appSettings.get('general.enableHomeScan')) {
        appSettings.set('general.enableHomeScan', true)
        appSettings.set('general.enableShowReplays', true)
        appSettings.set('general.enableShowFans', true)
        appSettings.set('general.enableShowFollowings', true)
    }

    if (!appSettings.get('history.viewed_maxage')) {
        appSettings.set('history', {
            viewed_maxage: 1
        })
    }

    let test = appSettings.get('position')
    if (test.mainWindow[1] === undefined) {
        appSettings.set('position', {
            mainWindow: [-1, -1],
            playerWindow: [-1, -1],
            bookmarksWindow: [-1, -1]
        })
    }

    let winsize = appSettings.get('size.mainWindow')

    mainWindow = defaultWindow(1024, 600);
    //mainWindow = defaultWindow(winsize[0],winsize[1]);

    mainWindow.resizable = false;




    wizardWindow = defaultWindow(520, 300);


    /**
     * Configure our window contents and callbacks
     */
    mainWindow.loadURL(`file://${__dirname}/loading.html`)
    mainWindow
        .on('close', () => {
            appSettings.set('position.mainWindow', mainWindow.getPosition())
            appSettings.set('size.mainWindow', mainWindow.getSize())

            if (playerWindow != null) {
                playerWindow.close()
            }
            if (bookmarksWindow != null) {
                bookmarksWindow.close()
            }
            if (chatWindow != null) {
                chatWindow.close()
            }

            mainWindow.webContents.session.clearCache(() => {
                // Purge the cache to help avoid eating up space on the drive
            })

            mainWindow = null

            setTimeout(() => {
                app.quit()
            }, 500)
        })

    wizardWindow.on('close', () => {
        wizardWindow.webContents.session.clearCache(() => {
            // Purge the cache to help avoid eating up space on the drive
        })

        if (mainWindow != null) {
            setTimeout(() => {
                let pos = appSettings.get('position.mainWindow')
                mainWindow.setPosition(pos[0], pos[1], false)
                mainWindow.show()
            }, 250)

        }

        wizardWindow = null

    })

    /**
     * Build our application menus using the templates provided
     * further down.
     */
    menu = Menu.buildFromTemplate(getMenuTemplate())
    Menu.setApplicationMenu(menu)

    coregateway.startCoreDetectionCycle();

    if (isFreshInstall) {
        wizardWindow.loadURL(`${HOST}/wizard.html`)
        wizardWindow.show()
    } else {
        mainWindow.show()
        mainWindow.loadURL(`file://${__dirname}/loading.html`)
        setTimeout(async () => {
            let rs = await startedcore;
            mainWindow.webContents.send('popup-message', { text: `LMPT Core: ` + rs })

        }, 2000)

        let pos = appSettings.get('position.mainWindow').length > 1 ? appSettings.get('position.mainWindow') : [null, null]
        if (pos[0] != null) {
            mainWindow.setPosition(pos[0], pos[1], false)
        }
    }
}

let shouldQuit = app.makeSingleInstance(function (commandLine, workingDirectory) {
    if (mainWindow) {
        mainWindow.focus()
    }
})
if (shouldQuit) {
    app.quit()
    process.exit()
}

app.on('ready', () => {
    createMainOrWizardWindow()
})

app.on('window-all-closed', () => {
    app.quit()
})

app.on('activate', () => {
    if (mainWindow === null) {
        createMainOrWizardWindow()
    }
})

/**
 * IPC Event Handlers
 */

ipcMain.on('open-home-window', (event, arg) => {
    var homeWindow = defaultWindow(400, 720);
    homeWindow.show()
    homeWindow.loadURL(`${HOST}/bookmarkScanner`)
})

ipcMain.on('open-core-control', (event, arg) => {

    var coreControlWindow = defaultWindow(800, 480);
    coreControlWindow.show()
    coreControlWindow.loadURL(`${HOST}/coreControl`)
})



/**
 * Watch a Replay - Use either internal player or external depending on settings
 */
ipcMain.on('watch-replay', (event, arg) => {

    let playerpath = appSettings.get('general.playerpath')

    if (playerpath.length > 5) {
        exec(playerpath.replace('%url%', video.hlsvideosource))
    } else {
        // Open internal player
        if (playerWindow == null) {
            let winposition = appSettings.get('position.playerWindow')
            let winsize = appSettings.get('size.playerWindow')

            playerWindow = defaultWindow(winsize[0], winsize[1]);
            // playerWindow.x = winposition[0] !== -1 ? winposition[0] : null
            // playerWindow.y = winposition[1] !== -1 ? winposition[1] : null

            playerWindow.on('close', () => {
                appSettings.set('position.playerWindow', playerWindow.getPosition())
                appSettings.set('size.playerWindow', playerWindow.getSize())

                playerWindow.webContents.session.clearCache(() => {
                    // Purge the cache to help avoid eating up space on the drive
                })
                playerWindow = null
            })
        }
        playerWindow.show()
        playerWindow.loadURL(`${HOST}/player/${arg.videoid}`)

    }
})


ipcMain.on('open-followings-window', (event, arg) => {

    let winposition = appSettings.get('position.followingsWindow') ? appSettings.get('position.followingsWindow') : [-1, -1]

    let win = defaultWindow(420, 720);
    // win.x = winposition[0] !== -1 ? winposition[0] : null
    // win.y = winposition[1] !== -1 ? winposition[1] : null

    win.show()
    win.on('close', () => {
        appSettings.set('position.followingsWindow', win.getPosition())
    })
    win.loadURL(`${HOST}/listwindow/followings/` + arg.userid)
})

ipcMain.on('open-followers-window', (event, arg) => {
    let winposition = appSettings.get('position.fansWindow') ? appSettings.get('position.fansWindow') : [-1, -1]

    let win = defaultWindow(420, 720);
    // win.x = winposition[0] !== -1 ? winposition[0] : null
    // win.y = winposition[1] !== -1 ? winposition[1] : null
    win.show()
    win.on('close', () => {
        appSettings.set('position.fansWindow', win.getPosition())
    })
    win.loadURL(`${HOST}/listwindow/fans/` + arg.userid)
})

ipcMain.on('read-comments', (event, arg) => {
    let win = defaultWindow(400, 600);

    win.on('ready-to-show', () => {
        win.showInactive()
    }).loadURL(`${HOST}/comments.html?` + arg.userid)
})

ipcMain.on('open-bookmarks', (event, arg) => {
    if (bookmarksWindow == null) {
        let winposition = appSettings.get('position.bookmarksWindow')
        let winsize = appSettings.get('size.bookmarksWindow')
        bookmarksWindow = defaultWindow(440, winsize[1]);
        bookmarksWindow.x = winposition[0] > -1 ? winposition[0] : null
        bookmarksWindow.y = winposition[1] > -1 ? winposition[1] : null

        bookmarksWindow.on('close', () => {
            appSettings.set('position.bookmarksWindow', bookmarksWindow.getPosition())
            appSettings.set('size.bookmarksWindow', bookmarksWindow.getSize())

            bookmarksWindow.webContents.session.clearCache(() => {
                // Purge the cache to help avoid eating up space on the drive
            })
            bookmarksWindow = null
        })
    } else {
        bookmarksWindow.restore()
    }

    bookmarksWindow.show()
    bookmarksWindow.loadURL(`${HOST}/bookmarks`)
})


function getMenuTemplate() {
    let template = [{
        label: 'Edit',
        submenu: [
            { role: 'undo' },
            { role: 'redo' },
            { type: 'separator' },
            { role: 'cut' },
            { role: 'copy' },
            { role: 'paste' },
            { role: 'delete' },
            { role: 'selectall' }
        ]
    },
    {
        role: 'window',
        submenu: [
            { role: 'minimize' },
            { role: 'close' },
            { type: 'separator' },
            {
                label: 'Developer Tools',
                submenu: [
                    { role: 'reload' },
                    { role: 'forcereload' },
                    { role: 'toggledevtools' }
                ]
            }
        ]
    },
    {
        role: 'help',
        submenu: [{
            label: 'LiveMe Pro Tools Page',
            click: () => shell.openExternal('https://github.com/lewdninja/liveme-pro-tools/')
        }]
    }
    ]

    /**
     * This is here in case macOS version gets added back end after all the bugs/issues are figured out.
     * Requires a contributor running macOS now.
     */
    if (process.platform === 'darwin') {
        template.unshift({
            label: appName,
            submenu: [{
                label: 'About ' + appName,
                click: () => { }
            },
            { type: 'separator' },
            { role: 'services', submenu: [] },
            { type: 'separator' },
            { role: 'hide' },
            { role: 'hideothers' },
            { role: 'unhide' },
            { type: 'separator' },
            {
                label: 'Quit ' + appName,
                accelerator: 'CommandOrControl+Q',
                click: () => { mainWindow.close() }
            }
            ]
        })
    }
    return template
}

function getMiniMenuTemplate() {
    let template = [{
        label: 'Edit',
        submenu: [
            { role: 'undo' },
            { role: 'redo' },
            { type: 'separator' },
            { role: 'cut' },
            { role: 'copy' },
            { role: 'paste' },
            { role: 'delete' },
            { role: 'selectall' }
        ]
    },
    {
        role: 'window',
        submenu: [
            { role: 'minimize' },
            { role: 'close' },
            { type: 'separator' },
            {
                label: 'Developer Tools',
                submenu: [
                    { role: 'reload' },
                    { role: 'forcereload' },
                    { role: 'toggledevtools' }
                ]
            }
        ]
    }
    ]
    return template
}