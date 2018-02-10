
function init_uploader(options) {
    var policyText = {
        "expiration": "2020-01-01T12:00:00.000Z", //设置该Policy的失效时间，超过这个失效时间之后，就没有办法通过这个policy上传文件了
        "conditions": [
        ["content-length-range", 0, 1048576000] // 设置上传文件的大小限制
        ]
    };

    //accessid = 'gkxXRp3Ny4gjMXjU';
    //accesskey = 'J0UTTIOAJwi8S7yLAO9aAeO8ACe23G';
    //host = 'http://vgic.oss-cn-beijing.aliyuncs.com';zh
    accessid = 'OoJldDR2Tld8r3AD';
    accesskey = 'oN9HwWhhqLtzVcGjPrfZ8USKX5kG3C';
    host = 'http://vgic-max.oss-cn-beijing.aliyuncs.com';

    var policyBase64 = Base64.encode(JSON.stringify(policyText))
    message = policyBase64
    var bytes = Crypto.HMAC(Crypto.SHA1, message, accesskey, { asBytes: true });
    var signature = Crypto.util.bytesToBase64(bytes);
    var uploader = new plupload.Uploader({
        runtimes: 'html5,flash,silverlight,html4',
        browse_button: 'selectfiles',
        //runtimes : 'flash',
        container: document.getElementById('upload-container'),
        flash_swf_url: 'lib/plupload-2.1.2/js/Moxie.swf',
        silverlight_xap_url: 'lib/plupload-2.1.2/js/Moxie.xap',

        url: host,

        multipart_params: {
            'Filename':  '${filename}',
            'key': options.filepath + '${filename}',
            'policy': policyBase64,
            'OSSAccessKeyId': accessid,
            'success_action_status': '200', //让服务端返回200,不然，默认会返回204
            'signature': signature,
        },

        init: {
            PostInit: function () {
                document.getElementById('ossfile').innerHTML = '';
            },

            FilesAdded: function (up, files) {
                plupload.each(files, function (file) {
                    var strArr =  file.name.split('_');
                    if (strArr.length < 4)
                    {
                        alert("文件命名不正确!");
                        return;
                    }

                    var item = $('<div id="' + file.id + '" class="col-md-12 upload-item">' + file.name + ' (' + plupload.formatSize(file.size) + ')<b></b>'
                    + '<div class="progress"><div class="progress-bar" style="width: 0%;text-align:right;"></div></div>'
                    + '</div>');
                    var close = $("<img />").addClass("close-img").attr("src", "../../Content/images/uploadify-cancel.png")
                        .appendTo(item);
                    close.click(function () {
                        item.remove();
                    })
                    $("#ossfile").append(item);
                });
            },

            UploadProgress: function (up, file) {
                var d = document.getElementById(file.id);
                d.getElementsByClassName('progress-bar')[0].innerHTML = '<span>' + file.percent + "%</span>";

                var prog = d.getElementsByTagName('div')[0];
                var progBar = prog.getElementsByTagName('div')[0]
                progBar.style.width =  file.percent + '%';
                progBar.setAttribute('aria-valuenow', file.percent);
            },

            FileUploaded: function (up, file, info) {
                var fileName = up.settings.multipart_params.key.replace("${filename}", file.name);
                var filepath = "";
                if (fileName.lastIndexOf("/") >= 0) {
                    filepath = fileName.substr(0, fileName.lastIndexOf("/") + 1);
                    fileName = fileName.substr(fileName.lastIndexOf("/") + 1);
                }

                if (info.status == 200) {
                    document.getElementById(file.id).getElementsByTagName('b')[0].innerHTML = '上传成功';
                    //保存文件信息
                    var args = {};
                    args.filepath = filepath;
                    args.fileName = fileName;
                    options.uploaded(args);
                }
                else {
                    document.getElementById(file.id).getElementsByTagName('b')[0].innerHTML = info.response;
                }
            },

            Error: function (up, err) {
                document.getElementById('console').appendChild(document.createTextNode("\nError xml:" + err.response));
            }
        }
    });

    uploader.init();

    return uploader;
}
