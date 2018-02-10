/*
 * 搜索区域 js
 */
$(function () {
    var searchArea = $('#searchArea');
    var searchDealerGroup = $('#searchDealerGroup');
    var searchDealer = $('#searchDealer');
    var searchComm = $('#searchArea, #searchDealerGroup, #searchDealer');
    var searchDate = $('#searchDate');

    var nH = (searchDate.find('div').height() - 20) + 'px';
    searchDate.find("p.ttl").css({ 'height': nH, 'lineHeight': nH });

    //显示/隐藏切换按钮
    searchComm.find('.switch').hover(function () {
        $(this).css('background', 'rgba(0,0,0,0.6)').find('a').show().css('top', ($(this).height() - 20) / 2);
    }, function () {
        $(this).css('background', '#fff').find('a').hide();
    });

    //切换到集团
    $('#toGroup').click(function () {
        //清空所有条件
        searchComm.find('li').attr('bChk', 2).removeClass("active").find('input').removeAttr('checked');
        $('#inputBox').html('');
        searchDealer.find('ul').html('');

        //切换到集团
        searchDealerGroup.show();
        searchArea.hide();

        heightHandle(searchDealerGroup.find('p.ttl'));
        heightHandle(searchDealer.find('p.ttl'));
    });

    //切换到区域
    $('#toArea').click(function () {
        //清空所有条件
        searchComm.find('li').attr('bChk', 2).removeClass("active").find('input').removeAttr('checked');
        $('#inputBox').html('');
        searchDealer.find('ul').html('');

        //切换到集团
        searchArea.show();
        searchDealerGroup.hide();

        heightHandle(searchDealerGroup.find('p.ttl'));
        heightHandle(searchDealer.find('p.ttl'));
    });

    //区域点击
    searchArea.find('li').click(function () {
        checkedObj($(this), true, true);

        if ($(this).attr('bChk') == 1) {
            getDealer($(this).find('input').val(), 1);
        }
        else {
            searchDealer.find('ul').html('');
        }
        updChkedInput();
        return false;
    });

    //集团点击
    searchDealerGroup.find('li').click(function () {
        checkedObj($(this), false, true);

        if ($(this).attr('bChk') == 1) {
            getDealer($(this).find('input').val(), 2);
        }
        else {
            //移除经销商
            searchDealer.find('li[group="' + $(this).find('input').val() + '"]').remove();
            //调整高度
            heightHandle(searchDealer.find('p.ttl'));

            if (searchDealerGroup.find('li[bChk=1]').length <= 0) {
                searchDealer.hide().find('ul').html('');
            }
        }
        updChkedInput();
        return false;
    });

    //时间点击
    //searchDate.find('select').change(function(){
    //	updChkedInput();
    //});

    searchDate.find('li').click(function () {
        checkedObj($(this), true, false);

        updChkedInput();
        return false;
    });
    //searchDate.find('li.currLI').click();		//当前月li点击

    //清空
    $('#clear').click(function () {
        $('#searchArea, #searchDealerGroup, #searchDealer,#searchDate').find('input[checked]').each(function () {
            $(this).removeAttr('checked').parent().attr('bChk', 2).removeClass("active");

        });
        updChkedInput();
    });
});

function checkedObj(obj, single, cancel) {
    if ($(obj).attr('bChk') == 2) {
        if (single) {
            $(obj).siblings('li').attr('bChk', 2).removeClass("active").find('input').removeAttr('checked');
        }
        $(obj).attr('bChk', 1).addClass("active").find('input').attr('checked', true);
    }
    else {
        if (cancel) {
            $(obj).attr('bChk', 2).removeClass("active").find('input').removeAttr('checked');
        }
    }
}

/*
 * 经销商点击
 */
function dealerLiClk(obj) {
    checkedObj(obj, false, true);

    updChkedInput();
    return false;
}

/*
 * 根据区域、集团获得经销商
 * @param 			sValue  		指定区域、集团的值
 * @param 			nSort  			1-区域， 2-集团
 */
function getDealer(sValue, nSort) {
    if (sValue == '' || sValue == 'undefined') {
        alert('无法获取经销商，请检查！');
        return false;
    }

    $.ajax({
        url: '../Common/SearchShop',
        type: 'get',
        async: false,
        data: { nSort: nSort, sValue: sValue },
        success: function (data) {
            var searchDealer = $('#searchDealer');
            var dealerUl = searchDealer.show().find('ul');
            if (nSort == 1) {
                dealerUl.empty();
                $.each(data, function (index, item) {
                    var li = $("<li bChk='2' onclick='dealerLiClk(this)'></li>").html(item.ShopName).append($("<input type='checkbox' name='nDealerID[]' value='" + item.ShopCode + "'>"));
                    dealerUl.append(li);
                });
            }
            else {
                $.each(data, function (index, item) {
                    var li = $("<li bChk='2'  group='" + sValue + "' onclick='dealerLiClk(this)'></li>").html(item.ShopName).append($("<input type='checkbox' name='nDealerID[]' value='" + item.ShopCode + "'>"));
                    dealerUl.append(li);
                });
            }

            heightHandle(searchDealer.find('p.ttl'));
        },
        error: function () {
            alert('获取经销商失败，请检查！');
        }
    });
};

/*
 * 获取所有已选条件，并追加到form
 */
function updChkedInput() {
    $('#inputBox').html('');

    $('div.searchComm ul li input[checked], #searchDate select').each(function () {
        $('#inputBox').append('<input type="hidden" name="' + $(this).attr('name') + '" value="' + $(this).val() + '" />');
    });
}

/*
 * 高度调整
 */
function heightHandle(objs) {
    objs.each(function () {
        var nH = $(this).siblings('ul').height() + 'px';
        $(this).css({ 'height': nH, 'lineHeight': nH });
    });
}

//字符产格式
function ChangeDateFormat(d) {
    if (!d) return "";
    var date = new Date(parseInt(d.replace("/Date(", "").replace(")/", ""), 10));
    var month = padLeft(date.getMonth() + 1, 10);
    var currentDate = padLeft(date.getDate(), 10);
    var hour = padLeft(date.getHours(), 10);
    var minute = padLeft(date.getMinutes(), 10);
    return date.getFullYear() + "-" + month + "-" + currentDate;// + " " + hour + ":" + minute;
}
function padLeft(str, min) {
    if (str >= min)
        return str;
    else
        return "0" + str;
}

//字符串拼接
function stringFormat() {
    if (arguments.length == 0)
        return null;

    var str = arguments[0];
    for (var i = 1; i < arguments.length; i++) {
        var re = new RegExp('\\{' + (i - 1) + '\\}', 'gm');
        str = str.replace(re, arguments[i]);
    }
    return str;
}


jQuery.Hashtable = function () {
    this.items = new Array();
    this.itemsCount = 0;
    this.add = function (key, value) {
        if (!this.containsKey(key)) {
            this.items[key] = value;
            this.itemsCount++;
        }
        else {
            throw "key '" + key + "' allready exists.";
        }
    };

    this.get = function (key) {
        if (this.containsKey(key)) {
            return this.items[key];
        }
        else {
            return null;
        }
    };

    this.remove = function (key) {
        if (this.containsKey(key)) {
            delete this.items[key];
            this.itemsCount--;
        }
        else {
            throw "key '" + key + "' does not exists.";
        }
    };

    this.containsKey = function (key) {
        return typeof (this.items[key]) != "undefined";
    };

    this.containsValue = function containsValue(value) {
        for (var item in this.items) {
            if (this.items[item] == value) {
                return true;
            }
        }
        return false;
    };

    this.contains = function (keyOrValue) {
        return this.containsKey(keyOrValue) || this.containsValue(keyOrValue);
    };

    this.clear = function () {
        this.items = new Array();
        itemsCount = 0;
    };

    this.size = function () {
        return this.itemsCount;
    };

    this.isEmpty = function () {
        return this.size() == 0;
    };

    this.getKey = function () {
        aKeyName = new Array();
        var nIndex = 0;
        for (var sKeyName in this.items) {
            aKeyName[nIndex] = sKeyName;
            nIndex++;
        }

        return aKeyName;
    };
};


//根据ID获取指定元素当前值
//仅支持radio，checkbox，text/textare，如果不是这三种类型，则无法预期运行结果。
//注意：传入ID参数时，不要传入前面的#号。
function getValueByID(sID) {
    var obj = $("#" + sID);
    if (typeof (obj) != "undefined") {
        //type为radio的情形
        if (obj.attr("type") == "radio") {
            var sName = sID.substring(3);
            var sValue = $("input[name='" + sName + "']:checked").val();
            if (typeof (sValue) != "undefined") {
                return sValue;
            }
            else {
                return "";
            }
        }
        else {
            //type为checkbox的情形
            if (obj.attr("type") == "checkbox") {
                var sName = sID.substring(3) + "[]";
                var sValue = "";
                $("input[name='" + sName + "']:checked").each(function () {
                    sValue = sValue + $(this).val() + ",";
                });
                if (sValue.length > 1) {
                    sValue = sValue.substring(0, sValue.lastIndexOf(","));
                }
                return sValue;
            }
            else {
                //type为其他（预期为text/textarea）的情形
                if (typeof (obj.val()) != "undefined") {
                    return obj.val();
                }
                else {
                    return "";
                }
            }
        }
    }
    else {
        return "";
    }
}
//存储格式检查正则表达式的 Hashtable
var hChkFormat = new jQuery.Hashtable();

//修改检查格式正则表达式 Hashtable的函数。
function changeChkFormat(sElementID, nNullFlag, sRegex, sChkHint, sFormID) {
    var aTmp = new Array(2);
    aTmp[0] = nNullFlag;
    aTmp[1] = sRegex;
    aTmp[2] = sChkHint;
    aTmp[3] = sFormID;
    //var sKey = sFormID + "|" + sElementID;
    //hChkFormat.add(sKey, aTmp);
    hChkFormat.add(sElementID, aTmp);
    //alert(hChkFormat.size());
}

//检查格式函数
function chkFormat(sFormID) {
    var bReturn = true;
    var aKeyName = hChkFormat.getKey();
    for (nAKeyNameIdx = 0; nAKeyNameIdx < aKeyName.length; nAKeyNameIdx++) {
        var sElementID = aKeyName[nAKeyNameIdx];
        var aTmp = hChkFormat.get(sElementID);
        if (sFormID == aTmp[3]) {
            var bPerformChkFlag = true;
            var sElementValue = getValueByID(sElementID);
            if (aTmp[0] == "1" && sElementValue == "") {
                bPerformChkFlag = false;
            }

            if (bPerformChkFlag == true) {
                var regExp = new RegExp(aTmp[1]);
                if (regExp.test(sElementValue) == false) {
                    bReturn = false;
                    $("#" + sElementID + "_chkHint").text(aTmp[2]);
                }
                else {
                    $("#" + sElementID + "_chkHint").text("");
                }
            }
        }
    }

    return bReturn;
}

/**
 * 前四个变量为固定的，必须传送，从第5个开始，可选。第5个参数为提交时的状态提示语
 * 本函数适用于提交不带文件上传的Form
 */
//开始函数 submitForm
function submitForm(joForm, sURL, sSubmitValue, sHintEID) {
    var joHint = $('#' + sHintEID);
    var sSubmitHint = '正在提交，请稍候...';
    if (arguments.length > 4) {
        sSubmitHint = arguments[4];
    }
    joHint.text(sSubmitHint);
    joHint.show();

    var sData = joForm.serialize() + "&sSubmit=" + sSubmitValue;
    var insArtSubmitReq = $.ajax({ async: true, cache: false, dataType: "html", url: sURL, data: sData, type: "post" });
    insArtSubmitReq.done(function (msg) {
        var oMsg = null;
        try {
            oMsg = $.parseJSON(msg);
        }
        catch (e) {
            alert(e);
            alert(msg);
        }

        if (oMsg != null) {
            //如果不跳转，则给出提示
            if (oMsg.nAction != 2) {
                var sHint = oMsg.sHint;
                if (oMsg.sErrMsg != "") {
                    sHint += '[参考错误消息：' + oMsg.sErrMsg + ']';
                }
                joHint.text(sHint);
                setTimeout(function () { joHint.fadeOut(2000); }, 5000);
            }

            switch (oMsg.nAction) {
                case 1:
                    {
                        break;
                    }
                case 2:
                    {
                        var sRedirectURL = oMsg.sRedirectURL;
                        if (sRedirectURL == "") {
                            sRedirectURL = "/";
                        }
                        window.location.href = sRedirectURL;
                        break;
                    }
                case 3:
                    {
                        joForm[0].reset();
                        break;
                    }
                default:
                    {
                        window.location.href = "/";
                    }
            }
        }

    });
}
//结束函数 submitForm

window.alert = function (msg,call) {
    if (!$('#myModal').html()) {
        $('body').append('<div id="myModal" class="modal fade"><div class="modal-dialog"><div class="modal-content"><div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button><h4 class="modal-title">提示</h4></div><div class="modal-body"><p id="msg"></p></div><div class="modal-footer"><button type="button" class="btn btn-default" data-dismiss="modal">确定</button></div></div></div></div>');
    }
    $('#msg').html(msg);
    $('#myModal').modal('show');
    $('#myModal').on('hidden.bs.modal', function (e) {
        if (call) call();
    })
}
window.closealert = function () {
    $('#myModal').modal('hide');
}