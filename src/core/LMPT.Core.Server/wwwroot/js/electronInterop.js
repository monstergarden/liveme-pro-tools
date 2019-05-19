




$(function () {
    document.title = 'LiveMe Pro Tools';
    // setupIPCListeners() // Set up our IPC listeners
    //setupContextMenu() // Set up the Context Menu for some UI elements
});


window.onerror = function myErrorHandler(errorMsg, url, lineNumber) {
    alert("Error occured: " + errorMsg);//or any message
    return false;
};




function minimizeWindow() { remote.BrowserWindow.getFocusedWindow().minimize() }
function closeWindow() { window.close() }
function showFollowing(u) { ipcRenderer.send('open-followings-window', { userid: u }) }
function showFollowers(u) { ipcRenderer.send('open-followers-window', { userid: u }) }
function openBookmarks() { ipcRenderer.send('open-bookmarks') }
function openHomeWindow() { ipcRenderer.send('open-home-window') }
function openCoreControl() { ipcRenderer.send('open-core-control') }
function playVideo(vid) { ipcRenderer.send('watch-replay', { videoid: vid }) }
function openURL(u) { var win = window.open(u, '_blank');
if (win) {
    //Browser has allowed it to be opened
    win.focus();
} else {
    //Browser has blocked it
    alert('Please allow popups for this website');
} }
function readComments(u) { ipcRenderer.send('read-comments', { userid: u }) }
function copyToClipboard(i) { clipboard.writeText(i) }
function quit() { remote.app.quit()}

// function openReplayContextMenu(vid) {
//     let replay = allReplays.find(r => r.vid === vid)
//     const replayContextMenu = remote.Menu.buildFromTemplate([{
//             label: 'Copy ID to Clipboard',
//             click: () => { copyToClipboard(vid) }
//         }, {
//             label: 'Copy Web URL to Clipboard',
//             click: () => { copyToClipboard(`https://www.liveme.com/live.html?videoid=${vid}`) }
        
//         }, {
//             label: 'Copy Source to Clipboard (m3u8 or flv)',
//             click: () => copyToClipboard(`${replay.videosource || replay.hlsvideosource}`)
//         }, {
//             label: 'Read Comments',
//             click: () => readComments(replay.vid)
//         }

//     ])
//     replayContextMenu.popup(remote.getCurrentWindow())
// }

// function setupContextMenu() {
//     const InputMenu = remote.Menu.buildFromTemplate([{
//         label: 'Undo',
//         role: 'undo'
//     }, {
//         label: 'Redo',
//         role: 'redo'
//     }, {
//         type: 'separator'
//     }, {
//         label: 'Cut',
//         role: 'cut'
//     }, {
//         label: 'Copy',
//         role: 'copy'
//     }, {
//         label: 'Paste',
//         role: 'paste'
//     }, {
//         type: 'separator'
//     }, {
//         label: 'Select all',
//         role: 'selectall'
//     }])

//     document.body.addEventListener('contextmenu', (e) => {
//         e.preventDefault()
//         e.stopPropagation()

//         let node = e.target

//         while (node) {
//             if (node.nodeName.match(/^(input|textarea)$/i) || node.isContentEditable) {
//                 InputMenu.popup(remote.getCurrentWindow())
//                 break
//             }
//             node = node.parentNode
//         }
//     })

//     const CopyableContextMenu = remote.Menu.buildFromTemplate([{
//         label: 'Copy',
//         role: 'copy'
//     }, {
//         label: 'Select all',
//         role: 'selectall'
//     }])

//     document.getElementById("username").addEventListener('contextmenu', (e) => {
//         e.preventDefault()
//         e.stopPropagation()
//         CopyableContextMenu.popup(remote.getCurrentWindow())
//     })
// }




// // TODO migrate rest to core later

// function setupIPCListeners() {

//     ipcRenderer.on('popup-message', (event, arg) => {
//         let p = $('#popup-message')
//         p.html(arg.text).animate({ top: 40 }, 400).delay(3000).animate({ top: 0 - p.height() }, 400)
//     })

//     ipcRenderer.on('download-start', (event, arg) => {
//         if ($('#download-' + arg.videoid).length < 1) return

//         $('#download-' + arg.videoid).addClass('active')
//         $('#download-' + arg.videoid + ' .status').html('Starting download..')
//         $('#download-' + arg.videoid + ' .filename').html(arg.filename)
//         $('#download-' + arg.videoid + ' .cancel').remove()
//     })

//     ipcRenderer.on('download-progress', (event, arg) => {
//         if ($('#download-' + arg.videoid).length < 1) return

//         $('#download-' + arg.videoid + ' .status').html(arg.state)
//         $('#download-' + arg.videoid + ' .progress-bar .bar').css('width', arg.percent + '%')
//     })

//     ipcRenderer.on('download-complete', (event, arg) => {
//         if ($('#download-' + arg.videoid).length < 1) return
//         $('#download-' + arg.videoid).remove()
//     })

//     ipcRenderer.on('download-error', (event, arg) => {
//         if ($('#download-' + arg.videoid).length < 1) return
//         $('#download-' + arg.videoid + ' .status').html('Download Error<span></span>')
//         $('#download-' + arg.videoid).append(`<div onClick="cancelDownload('${arg.videoid}')" class="cancel">&#x2715;</div>`)
//     })
// }

// function sortReplays(name) {
//     // $('table#list tbody tr').sort(function(a, b) {
//     //     var aValue = $(a).find('td[data-name="' + name + '"]').text()
//     //     var bValue = $(b).find('td[data-name="' + name + '"]').text()
//     //     if (name === 'date') {
//     //         aValue = new Date(aValue).getTime()
//     //         bValue = new Date(bValue).getTime()
//     //     }
//     //     return ((+aValue < +bValue) ? 1 : ((+aValue > +bValue) ? -1 : 0))
//     // }).appendTo('table#list tbody')
//     // if (hasMore) {
//     //     setTimeout(() => sortReplays(name), 500)
//     // }
// }

// function downloadVideo(vid) {
//     $('#download-replay-' + vid).html('<i class="icon icon-download dim"></i>')
//     $('#download-replay-' + vid).unbind()

//     // if (appSettings.get('lamd.handle_downloads') === true) {
//     //     AddReplayToLAMD(vid)
//     // } else {
//     //     ipcRenderer.send('download-replay', { videoid: vid })

//     //     if ($('#download-' + vid).length > 0) return
//     //     $('#queue-list').append(`
//     //         <div class="download" id="download-${vid}">
//     //             <div class="filename">${vid}</div>
//     //             <div class="status">Queued</div>
//     //             <div class="progress-bar">
//     //                 <div class="bar" style="width: 0%"></div>
//     //             </div>
//     //             <div onClick="cancelDownload('${vid}')" class="cancel">&#x2715;</div>
//     //         </div>
//     //     `)
//     // }
// }

// function cancelDownload(i) {
//     ipcRenderer.send('download-cancel', { videoid: i })
//     $('#download-' + i).remove()
// }

// function showDownloads() {
//     if ($('#queue-list').is(':visible')) {
//         $('overlay').hide()
//         $('#queue-list').hide()
//     } else {
//         $('overlay').show()
//         $('#queue-list').show()
//     }
// }




