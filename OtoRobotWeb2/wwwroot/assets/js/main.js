function buttonLoaderOpen() {
    $("#btn_sorgula").prop("disabled", true);
    // add spinner to button
    $("#btn_sorgula").html(
        '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Yukleniyor...'
    );
    document.getElementById("sorguDetay").className = "d-none"
}
function buttonLoaderClose() {
    $("#btn_sorgula").prop("disabled", false);
    // add spinner to button
    $("#btn_sorgula").html(
        'Sorgula'
    );
    document.getElementById("sorguDetay").className = ""

}