!function($,window,document){"use strict";function Wickedpicker(e,t){this.element=$(e),this.options=$.extend({},defaults,t),this.element.addClass("hasWickedpicker"),this.element.attr("onkeypress","return false;"),this.element.attr("aria-showingpicker","false"),this.createPicker(),this.timepicker=$(".wickedpicker"),this.up=$("."+this.options.upArrow),this.down=$("."+this.options.downArrow),this.separator=$(".wickedpicker__controls__control--separator"),this.hoursElem=$(".wickedpicker__controls__control--hours"),this.minutesElem=$(".wickedpicker__controls__control--minutes"),this.secondsElem=$(".wickedpicker__controls__control--seconds"),this.meridiemElem=$(".wickedpicker__controls__control--meridiem"),this.close=$("."+this.options.close);var i=this.timeArrayFromString(this.options.now);this.options.now=new Date(today.getFullYear(),today.getMonth(),today.getDate(),i[0],i[1],i[2]),this.selectedHour=this.parseHours(this.options.now.getHours()),this.selectedMin=this.parseSecMin(this.options.now.getMinutes()),this.selectedSec=this.parseSecMin(this.options.now.getSeconds()),this.selectedMeridiem=this.parseMeridiem(this.options.now.getHours()),this.setHoverState(),this.attach(e),this.setText(e)}"function"!=typeof String.prototype.endsWith&&(String.prototype.endsWith=function(e){return e.length>0&&this.substring(this.length-e.length,this.length)===e});var isMobile=function(){return/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)},today=new Date,pluginName="wickedpicker",defaults={now:today.getHours()+":"+today.getMinutes(),twentyFour:!1,upArrow:"wickedpicker__controls__control-up",downArrow:"wickedpicker__controls__control-down",close:"wickedpicker__close",hoverState:"hover-state",title:"Timepicker",showSeconds:!1,secondsInterval:1,minutesInterval:1,beforeShow:null,show:null,clearable:!1};$.extend(Wickedpicker.prototype,{showPicker:function(e){"function"==typeof this.options.beforeShow&&this.options.beforeShow(e,this.timepicker);var t=$(e).offset();if($(e).attr({"aria-showingpicker":"true",tabindex:-1}),this.setText(e),this.showHideMeridiemControl(),this.getText(e)!==this.getTime()){var i=this.getText(e).replace(/:/g,"").split(" "),s={};s.hours=i[0],s.minutes=i[2],this.options.showSeconds?(s.seconds=i[4],s.meridiem=i[5]):s.meridiem=i[3],this.setTime(s)}this.timepicker.css({"z-index":this.element.css("z-index")+1,position:"absolute",left:t.left,top:t.top+$(e)[0].offsetHeight}).show(),"function"==typeof this.options.show&&this.options.show(e,this.timepicker),this.handleTimeAdjustments(e)},hideTimepicker:function(e){function t(e){setTimeout(function(){$('[aria-showingpicker="false"]').attr("tabindex",e)},400)}this.timepicker.hide();var i={start:function(){var e=$.Deferred();return $('[aria-showingpicker="true"]').attr("aria-showingpicker","false"),e.promise()}};i.start().then(t(0))},createPicker:function(){if(0===$(".wickedpicker").length){var e='<div class="wickedpicker"><p class="wickedpicker__title">'+this.options.title+'<span class="wickedpicker__close"></span></p><ul class="wickedpicker__controls"><li class="wickedpicker__controls__control"><span class="'+this.options.upArrow+'"></span><span class="wickedpicker__controls__control--hours" tabindex="-1">00</span><span class="'+this.options.downArrow+'"></span></li><li class="wickedpicker__controls__control--separator"><span class="wickedpicker__controls__control--separator-inner">:</span></li><li class="wickedpicker__controls__control"><span class="'+this.options.upArrow+'"></span><span class="wickedpicker__controls__control--minutes" tabindex="-1">00</span><span class="'+this.options.downArrow+'"></span></li>';this.options.showSeconds&&(e+='<li class="wickedpicker__controls__control--separator"><span class="wickedpicker__controls__control--separator-inner">:</span></li><li class="wickedpicker__controls__control"><span class="'+this.options.upArrow+'"></span><span class="wickedpicker__controls__control--seconds" tabindex="-1">00</span><span class="'+this.options.downArrow+'"></span> </li>'),e+='<li class="wickedpicker__controls__control"><span class="'+this.options.upArrow+'"></span><span class="wickedpicker__controls__control--meridiem" tabindex="-1">AM</span><span class="'+this.options.downArrow+'"></span></li></ul></div>',$("body").append(e),this.attachKeyboardEvents()}},showHideMeridiemControl:function(){this.options.twentyFour===!1?$(this.meridiemElem).parent().show():$(this.meridiemElem).parent().hide()},showHideSecondsControl:function(){this.options.showSeconds?$(this.secondsElem).parent().show():$(this.secondsElem).parent().hide()},attach:function(e){var t=this;this.options.clearable&&t.makePickerInputClearable(e),$(e).attr("tabindex",0),$(e).on("click focus",function(e){$(t.timepicker).is(":hidden")&&(t.showPicker($(this)),$(t.hoursElem).focus())});var i=function(i){$(t.timepicker).is(":visible")&&($(i.target).is(t.close)?t.hideTimepicker(e):$(i.target).closest(t.timepicker).length||$(i.target).closest($(".hasWickedpicker")).length?i.stopPropagation():t.hideTimepicker(e))};$(document).off("click",i).on("click",i)},attachKeyboardEvents:function(){$(document).on("keydown",$.proxy(function(e){switch(e.keyCode){case 9:"hasWickedpicker"!==e.target.className&&$(this.close).trigger("click");break;case 27:$(this.close).trigger("click");break;case 37:e.target.className!==this.hoursElem[0].className?$(e.target).parent().prevAll("li").not(this.separator.selector).first().children()[1].focus():$(e.target).parent().siblings(":last").children()[1].focus();break;case 39:e.target.className!==this.meridiemElem[0].className?$(e.target).parent().nextAll("li").not(this.separator.selector).first().children()[1].focus():$(e.target).parent().siblings(":first").children()[1].focus();break;case 38:$(":focus").prev().trigger("click");break;case 40:$(":focus").next().trigger("click")}},this))},setTime:function(e){this.setHours(e.hours),this.setMinutes(e.minutes),this.setMeridiem(e.meridiem),this.options.showSeconds&&this.setSeconds(e.seconds)},getTime:function(){return[this.formatTime(this.getHours(),this.getMinutes(),this.getMeridiem(),this.getSeconds())]},setHours:function(e){var t=new Date;t.setHours(e);var i=this.parseHours(t.getHours());this.hoursElem.text(i),this.selectedHour=i},getHours:function(){var e=new Date;return e.setHours(this.hoursElem.text()),e.getHours()},parseHours:function(e){return this.options.twentyFour===!1?(e+11)%12+1:e<10?"0"+e:e},setMinutes:function(e){var t=new Date;t.setMinutes(e);var i=t.getMinutes(),s=this.parseSecMin(i);this.minutesElem.text(s),this.selectedMin=s},getMinutes:function(){var e=new Date;return e.setMinutes(this.minutesElem.text()),e.getMinutes()},parseSecMin:function(e){return(e<10?"0":"")+e},setMeridiem:function(e){var t="";if(void 0===e){var i=this.getMeridiem();t="PM"===i?"AM":"PM"}else t=e;this.meridiemElem.text(t),this.selectedMeridiem=t},getMeridiem:function(){return this.meridiemElem.text()},setSeconds:function(e){var t=new Date;t.setSeconds(e);var i=t.getSeconds(),s=this.parseSecMin(i);this.secondsElem.text(s),this.selectedSec=s},getSeconds:function(){var e=new Date;return e.setSeconds(this.secondsElem.text()),e.getSeconds()},parseMeridiem:function(e){return e>11?"PM":"AM"},handleTimeAdjustments:function(e){var t=0;$(this.up).add(this.down).off("mousedown click touchstart").on("mousedown click",{Wickedpicker:this,input:e},function(e){var i=this.className.indexOf("up")>-1?"+":"-",s=e.data;"mousedown"==e.type?t=setInterval($.proxy(function(e){e.Wickedpicker.changeValue(i,e.input,this)},this,{Wickedpicker:s.Wickedpicker,input:s.input}),200):s.Wickedpicker.changeValue(i,s.input,this)}).bind("mouseup touchend",function(){clearInterval(t)})},changeValue:function(operator,input,clicked){var target="+"===operator?clicked.nextSibling:clicked.previousSibling,targetClass=$(target).attr("class");targetClass.endsWith("hours")?this.setHours(eval(this.getHours()+operator+1)):targetClass.endsWith("minutes")?this.setMinutes(eval(this.getMinutes()+operator+this.options.minutesInterval)):targetClass.endsWith("seconds")?this.setSeconds(eval(this.getSeconds()+operator+this.options.secondsInterval)):this.setMeridiem(),this.setText(input)},setText:function(e){$(e).val(this.formatTime(this.selectedHour,this.selectedMin,this.selectedMeridiem,this.selectedSec)).change()},getText:function(e){return $(e).val()},formatTime:function(e,t,i,s){var n=e+" : "+t;return this.options.twentyFour&&(n=e+" : "+t),this.options.showSeconds&&(n+=" : "+s),this.options.twentyFour===!1&&(n+=" "+i),n},setHoverState:function(){var e=this;isMobile()||$(this.up).add(this.down).add(this.close).hover(function(){$(this).toggleClass(e.options.hoverState)})},makePickerInputClearable:function(e){$(e).wrap('<div class="clearable-picker"></div>').after("<span data-clear-picker>&times;</span>"),$("[data-clear-picker]").on("click",function(e){$(this).siblings(".hasWickedpicker").val("")})},timeArrayFromString:function(e){if(e.length){var t=e.split(":");return t[2]=t.length<3?"00":t[2],t}return!1},_time:function(){var e=$(this.element).val();return""===e?this.formatTime(this.selectedHour,this.selectedMin,this.selectedMeridiem,this.selectedSec):e}}),$.fn[pluginName]=function(e,t){return $.isFunction(Wickedpicker.prototype["_"+e])?$(this).hasClass("hasWickedpicker")?void 0!==t?$.data($(this)[t],"plugin_"+pluginName)["_"+e]():$.data($(this)[0],"plugin_"+pluginName)["_"+e]():void 0:this.each(function(){$.data(this,"plugin_"+pluginName)||$.data(this,"plugin_"+pluginName,new Wickedpicker(this,e))})}}(jQuery,window,document);