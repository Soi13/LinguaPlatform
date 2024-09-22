//Checking type of browser and if it Safari, show warning to user.
$(function () {
    var is_opera = !!window.opera || navigator.userAgent.indexOf(' OPR/') >= 0;
    var is_Edge = navigator.userAgent.indexOf("Edge") > -1;
    var is_chrome = !!window.chrome && !is_opera && !is_Edge;
    var is_explorer = typeof document !== 'undefined' && !!document.documentMode && !is_Edge;
    var is_firefox = typeof window.InstallTrigger !== 'undefined';
    var is_safari = /^((?!chrome|android).)*safari/i.test(navigator.userAgent);

    if (is_safari) {
        $.messager.alert('Attention', "Your browser is Safari. For more functionality of platform we are recommending to use another browser.", 'warning');
    }

});