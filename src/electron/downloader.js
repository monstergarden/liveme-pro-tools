const { app, BrowserWindow, ipcMain, Menu, shell, dialog } = require('electron')
const { exec } = require('child_process')
const fs = require('fs')
const path = require('path')

const request = require('request')
const ffmpeg = require('fluent-ffmpeg')
const async = require('async')

ipcMain.on('downloads-parallel', (event, arg) => {
    dlQueue.concurrency = arg
})



ipcMain.on('download-replay', (event, arg) => {
    DataManager.addToQueueList(arg.videoid)
    dlQueue.push(arg.videoid, err => {
        if (err) {
            mainWindow.webContents.send('download-error', err)
        } else {
            mainWindow.webContents.send('download-complete', { videoid: arg.videoid })
        }
    })
})
/**
 * Cannot cancel active download, only remove queued entries.
 */
ipcMain.on('download-cancel', (event, arg) => {
    dlQueue.remove(function(task) {
        if (task.data === arg.videoid) {
            DataManager.removeFromQueueList(task.data)
            return true
        }
        return false
    })
})
/**
 * It is done this way in case the API call to jDownloader returns an error or doesn't connect.
 */
const dlQueue = async.queue((task, done) => {
// Set custom FFMPEG path if defined
if (appSettings.get('downloads.ffmpeg')) ffmpeg.setFfmpegPath(appSettings.get('downloads.ffmpeg'))
    // Get video info
LiveMe.getVideoInfo(task).then(video => {
    const path = appSettings.get('downloads.path')
    const dt = new Date(video.vtime * 1000)
    const mm = dt.getMonth() + 1
    const dd = dt.getDate()
    let ffmpegOpts = []

    let filename = appSettings.get('downloads.template')
        .replace(/%%broadcaster%%/g, video.uname)
        .replace(/%%longid%%/g, video.userid)
        .replace(/%%replayid%%/g, video.vid)
        .replace(/%%replayviews%%/g, video.playnumber)
        .replace(/%%replaylikes%%/g, video.likenum)
        .replace(/%%replayshares%%/g, video.sharenum)
        .replace(/%%replaytitle%%/g, video.title ? video.title : 'untitled')
        .replace(/%%replayduration%%/g, video.videolength)
        .replace(/%%replaydatepacked%%/g, (dt.getFullYear() + (mm < 10 ? '0' : '') + mm + (dd < 10 ? '0' : '') + dd))
        .replace(/%%replaydateus%%/g, ((mm < 10 ? '0' : '') + mm + '-' + (dd < 10 ? '0' : '') + dd + '-' + dt.getFullYear()))
        .replace(/%%replaydateeu%%/g, ((dd < 10 ? '0' : '') + dd + '-' + (mm < 10 ? '0' : '') + mm + '-' + dt.getFullYear()))

    filename = filename.replace(/[/\\?%*:|"<>]/g, '-')
    filename = filename.replace(/([^a-z0-9\s]+)/gi, '-')
    filename = filename.replace(/[\u{0080}-\u{FFFF}]/gu, '')
    filename += '.mp4'
    video._filename = filename

    mainWindow.webContents.send('download-start', {
        videoid: task,
        filename: filename
    })

    switch (parseInt(appSettings.get('downloads.ffmpegquality'))) {
        case 9: // AMD Hardware HEVC/H265 Encoder
            ffmpegOpts = [
                '-c:v hevc_amf',
                '-preset superfast',
                '-b:v 300k',
                '-r 15',
                '-c:a copy',
                '-bsf:a aac_adtstoasc',
                '-vsync 2',
                '-movflags faststart'
            ]
            break

        case 8: // nVidia Hardware HEVC/H265 Encoder
            ffmpegOpts = [
                '-c:v hevc_nvenc',
                '-preset superfast',
                '-b:v 300k',
                '-r 15',
                '-c:a copy',
                '-bsf:a aac_adtstoasc',
                '-vsync 2',
                '-movflags faststart'
            ]
            break

        case 7: // Intel Hardware HEVC/H265 Encoder
            ffmpegOpts = [
                '-c:v hevc_qsv',
                '-preset superfast',
                '-b:v 300k',
                '-r 15',
                '-c:a copy',
                '-bsf:a aac_adtstoasc',
                '-vsync 2',
                '-movflags faststart'
            ]
            break

        case 6: // HEVC/H265 Encoder
            ffmpegOpts = [
                '-c:v hevc',
                '-preset superfast',
                '-b:v 300k',
                '-r 15',
                '-c:a copy',
                '-bsf:a aac_adtstoasc',
                '-vsync 2',
                '-movflags faststart'
            ]
            break

        case 5: // AMD AMF Hardware Enabled - Experimental
            ffmpegOpts = [
                '-c:v h264_amf',
                '-preset none',
                '-b:v 500k',
                '-r 15',
                '-c:a copy',
                '-bsf:a aac_adtstoasc',
                '-vsync 2',
                '-movflags faststart'
            ]
            break

        case 4: // nVidia Hardware Enabled - Experimental
            ffmpegOpts = [
                '-c:v h264_nvenc',
                '-preset none',
                '-b:v 500k',
                '-r 15',
                '-c:a copy',
                '-bsf:a aac_adtstoasc',
                '-vsync 2',
                '-movflags faststart'
            ]
            break

        case 3: // Intel Hardware Enabled - Experimental
            ffmpegOpts = [
                '-c:v h264_qsv',
                '-preset none',
                '-b:v 500k',
                '-r 15',
                '-c:a copy',
                '-bsf:a aac_adtstoasc',
                '-vsync 2',
                '-movflags faststart'
            ]
            break

        case 2: // Best
            ffmpegOpts = [
                '-c:v h264',
                '-preset fast',
                '-c:a copy',
                '-bsf:a aac_adtstoasc',
                '-vsync 2',
                '-movflags faststart'
            ]
            break

        case 1: // Fast
            ffmpegOpts = [
                '-c:v h264',
                '-preset superfast',
                '-q:v 0',
                '-c:a copy',
                '-bsf:a aac_adtstoasc',
                '-vsync 2',
                '-movflags faststart'
            ]
            break

        default: // None
            ffmpegOpts = [
                '-c copy',
                '-bsf:a aac_adtstoasc',
                '-vsync 2',
                '-movflags faststart'
            ]
            break
    }


    switch (appSettings.get('downloads.method')) {
        case 'chunk':
            request(video.hlsvideosource, (err, res, body) => {
                if (err || !body) {
                    fs.writeFileSync(`${path}/${filename}-error.log`, JSON.stringify(err, null, 2))
                    return done({ videoid: task, error: err || 'Failed to fetch m3u8 file.' })
                }
                // Separate ts names from m3u8
                let concatList = ''
                const tsList = []
                body.split('\n').forEach(line => {
                        if (line.indexOf('.ts') !== -1) {
                            const tsName = line.split('?')[0]
                            let tsPath = `${path}/lpt_temp/${video.vid}_${tsName}`

                            if (process.platform == 'win32') {
                                tsPath = tsPath.replace(/\\/g, '/');
                            }

                            // Check if TS has already been added to array
                            if (concatList.indexOf(tsPath) === -1) {
                                // We'll use this later to merge downloaded chunks
                                concatList += 'file ' + video.vid + '_' + tsName + '\n'
                                    // Push data to list
                                tsList.push({ name: tsName, path: tsPath })
                            }
                        }
                    })
                    // remove last |
                    //concatList = concatList.slice(0, -1)
                    // Check if tmp dir exists
                if (!fs.existsSync(`${path}/lpt_temp`)) {
                    // create temporary dir for ts files
                    fs.mkdirSync(`${path}/lpt_temp`)
                }
                fs.writeFileSync(`${path}/lpt_temp/${video.vid}.txt`, concatList)

                // Download chunks
                let downloadedChunks = 0

                async.eachLimit(tsList, 3, (file, next) => {

                    const stream = request(`${video.hlsvideosource.split('/').slice(0, -1).join('/')}/${file.name}`)
                        .on('error', err => {
                            fs.writeFileSync(`${path}/${filename}-error.log`, JSON.stringify(err, null, 2))
                            return done({ videoid: task, error: err })
                        })
                        .pipe(
                            fs.createWriteStream(file.path)
                        )
                        // Events
                    stream.on('finish', () => {
                        downloadedChunks += 1
                        mainWindow.webContents.send('download-progress', {
                            videoid: task,
                            state: `Downloading stream chunks.. (${downloadedChunks}/${tsList.length})`,
                            percent: Math.round((downloadedChunks / tsList.length) * 100)
                        })
                        next()
                    })

                }, () => {
                    // Chunks downloaded
                    let cfile = path + '/lpt_temp/' + video.vid + '.txt'
                    ffmpeg()
                        .on('start', c => {
                            mainWindow.webContents.send('download-progress', {
                                videoid: task,
                                state: `Converting to MP4 file, please wait..`,
                                percent: 0
                            })
                        })
                        .on('progress', function(progress) {
                            // FFMPEG doesn't always have this >.<
                            let p = progress.percent
                            if (p > 100) p = 100
                            mainWindow.webContents.send('download-progress', {
                                videoid: task,
                                state: `Combining and converting to MP4 file, please wait...`,
                                percent: p
                            })
                        })
                        .on('end', (stdout, stderr) => {
                            DataManager.addDownloaded(video.vid)
                            if (appSettings.get('downloads.deltmp')) {
                                tsList.forEach(file => fs.unlinkSync(file.path))
                            }
                            return done()
                        })
                        .on('error', (err) => {
                            fs.writeFileSync(`${path}/${filename}-error.log`, err)
                            return done({ videoid: task, error: err })
                        })
                        .input(cfile.replace(/\\/g, '/'))
                        .inputFormat('concat')
                        .output(`${path}/${filename}`)
                        .inputOptions([
                            '-safe 0',
                            '-f concat'
                        ])
                        .outputOptions(ffmpegOpts)
                        .run()
                })
            })
            break
        case 'ffmpeg':
            ffmpeg(video.hlsvideosource)
                .outputOptions(ffmpegOpts)
                .output(path + '/' + filename)
                .on('end', function(stdout, stderr) {
                    DataManager.addDownloaded(video.vid)
                    return done()
                })
                .on('progress', function(progress) {
                    // FFMPEG doesn't always have this >.<
                    if (!progress.percent) {
                        progress.percent = ((progress.targetSize * 1000) / +video.videosize) * 100
                    }
                    mainWindow.webContents.send('download-progress', {
                        videoid: task,
                        state: `Downloading (${Math.round(progress.percent)}%)`,
                        percent: progress.percent
                    })
                })
                .on('start', function(c) {
                    console.log('started', c)
                    mainWindow.webContents.send('download-start', {
                        videoid: task,
                        filename: filename
                    })
                })
                .on('error', function(err, stdout, stderr) {
                    fs.writeFileSync(`${path}/${filename}-error.log`, JSON.stringify([err, stdout, stderr], null, 2))
                    return done({ videoid: task, error: err })
                })
                .run()
            break
    }
})
}, +appSettings.get('downloads.parallel') || 3)
