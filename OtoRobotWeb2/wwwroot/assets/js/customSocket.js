var jsonRequestTime = { trafik: 2250, kasko: 2250, aracverdi: 2250 }


$(document).ready(function () {

    fnk_checkloginSocket();

    //Danger Alert Start
    const alertPlaceholder = document.getElementById('liveAlertPlaceholder')
    const appendAlert = (message) => {
        const wrapper = document.createElement('div')
        wrapper.innerHTML = [
            `<div class="alert alert-danger alert-dismissible" id="danger-alert" role="alert">`,
            `   <div>${message}</div>`,
            '   <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>',
            '</div>'
        ].join('')

        alertPlaceholder.append(wrapper)
    }
    //Danger Alert End
    //Parca arama
    $("#btn_sorgula").click(() => {
        let sData = document.getElementById("inputoem").value.trim();
        if (ListQueryStringFlexCheckDefault() != "" && sData.length > 3) {
            //tablo reset
            try {
                fnk_DataTable_Destroy();
            } catch (e) {
            }
            //document.querySelector("#table_offer").innerHTML = `
            //        <thead class="bg-light-blue text-white">
            //            <tr>
            //                <th scope="col">Tedarikçi Adı</th>
            //                <th scope="col">Stok</th>
            //                <th scope="col">Stok No</th>
            //                <th scope="col">OEM No</th>
            //                <th scope="col">Ürün Adı</th>
            //                <th scope="col">Marka</th>
            //                <th scope="col">Kdv'siz Fiyat</th>
            //                <th scope="col">Kdv'li Fiyat</th>
            //                <th scope="col">Liste Fiyatı</th>
            //                <th scope="col" class="text-end">Liste Tarihi</th>
            //            </tr>
            //        </thead>
            //        <tbody id="sorguParcaList">

            //        </tbody>`;
            document.querySelector("#table_offer").innerHTML = `
                    <thead class="bg-light-blue text-white">
                        <tr>
                            <th scope="col">Tedarikçi Adı</th>
                            <th scope="col">Stok</th>
                            <th scope="col">Stok No</th>
                            <th scope="col">OEM No</th>
                            <th scope="col">Ürün Adı</th>
                            <th scope="col">Marka</th>
                            <th scope="col">Kdv'siz Fiyat</th>
                            <th scope="col">Kdv'li Fiyat</th>
                            <th scope="col">Liste Fiyatı</th> 
                            <th scope="col">Fiyatlar</th> 
                        </tr>
                    </thead>
                    <tbody id="sorguParcaList">

                    </tbody>`;

            //yeni loader
            buttonLoaderOpen()

            fnk_sendOffer();
            /*$("#loaddiv").fadeOut();*/
        }
        else {
            if (ListQueryStringFlexCheckDefault() == "") {
                appendAlert('En az 1 Tedarikçi seçili olmalı')
            }

            if (!(sData.length > 3)) {
                appendAlert('Oem Numarası 3 karakterden kısa olamaz')
            }
        }
        //alert auto close
        window.setTimeout(function () {
            $(".alert-danger").alert('close');
        }, 5000);

    });

    $("#inputoem").keypress(function (e) {
        if (e.which == 13) {
            //$("#loaddiv").fadeIn();
            buttonLoaderOpen()
            fnk_sendOffer();
        }
    });
    //logout islemi
    $("#btn_logout").click(() => {
        $.ajax({
            type: "GET",
            url: "/Login/Logout/",
            success: (data) => {
                alert("Çıkış işlemi başarılı");
                window.location.href = "/Login/Index";
            },
            error: (xhr, status) => { },
            complete: (xhr, status) => {
            }
        });
    })
});

//screen Socket
function viewScreen(data) {
    //console.log(data);

    document.getElementById("div_img").innerHTML = `<img src="data:image/png;base64, ${data}" alt="..." class="img-fluid" />`
}
async function fnk_SendScreenSocket() {
    return new Promise((resolve, reject) => {
        $.ajax({
            type: "GET",
            url: "/api/request/sendscreen",
            success: (res) => {
                resolve(true)
            },
            error: (xhr, status) => {
            },
            complete: (xhr, status) => {
                reject(false)
            }
        });
    })
}
async function fnk_checkScreenSocket() {
    $.ajax({
        type: "GET",
        url: "/api/request/checkscreen",
        success: (res) => {
            viewScreen(res.data)
        },
        error: (xhr, status) => { },
        complete: (xhr, status) => { }

    });
}

// Oto Login islemleri
function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

async function fnk_SendLoginSocket() {
    return new Promise((resolve, reject) => {
        $.ajax({
            type: "GET",
            url: "/api/request/sendlogin",
            success: (res) => {
                resolve(true)
            },
            error: (xhr, status) => {
            },
            complete: (xhr, status) => {
                reject(false)
            }
        });
    })
}

async function fnk_checkloginSocket() {

    $.ajax({
        type: "GET",
        url: "/api/request/checklogin",
        success: (res) => {
            OtoRobotSirketler_View(res)
        },
        error: (xhr, status) => { },
        complete: (xhr, status) => {
            setTimeout(function () {
                fnk_checkloginSocket();
            }, 10000);
        }

    });
}
//async function fnk_relogin(sirketkodu) {
//    var data = { sirketkodu};
//    $.ajax({
//        type: "GET",
//        url: "/api/request/relogin",
//        data:data,
//        success: (res) => {
           
//        },
//        error: (xhr, status) => { },
//        complete: (xhr, status) => {
           
//        }

//    });
//}

function CreateGuid() {
    function _p8(s) {
        var p = (Math.random().toString(16) + "000000000").substr(2, 8);
        return s ? "-" + p.substr(0, 4) + "-" + p.substr(4, 4) : p;
    }
    return _p8() + _p8(true) + _p8(true) + _p8();
}
// Oto Sorgu islemleri
var checkSocketProcess = false;
async function fnk_sendOffer() {
    var guid = CreateGuid();
    let sData = document.getElementById("inputoem").value;
    fnk_DataTable_Destroy();
    document.querySelector("#sorguParcaList").innerHTML = "";
    document.getElementById("div_img").innerHTML = ""
    $.ajax({
        type: "GET",
        url: "/api/request/sendoffer?OemNumarasi=" + sData + "&StokVarmi=" + flexCheckDefault.checked + "&SorguId=" + guid + ListQueryStringFlexCheckDefault(),
        success: (data) => { },
        error: (xhr, status) => { },
        complete: (xhr, status) => {
            checkSocketProcess = false;
            setTimeout(function () {
                if (!checkSocketProcess) {
                    fnk_CheckOfferSorguSocket(guid);
                }
            }, 5000);
        }
    });

}

function ListQueryStringFlexCheckDefault() {
    var listDom = document.querySelectorAll("#flexSwitchCheckChecked");

    var result = "";
    if (listDom.length == 0) {
        listDom = document.querySelectorAll("#flexSwitchCheckChecked1")
        listDom.forEach(function (i, j) {
            result += "&SorgulanacakFirmalar=" + i.getAttribute("value");

        })
    }
    else {
        listDom.forEach((i) => {
            if (i.checked) {
                result += "&SorgulanacakFirmalar=" + i.value;
            }
        });
    }

    return result;
}//5G1941006E

function fnk_CheckOfferSorguSocket(guid) {
    let sData = document.getElementById("inputoem").value;
    $.ajax({
        type: "GET",
        url: "/api/request/checkoffer?SorguId=" + guid + "&OemNumarasi=" + sData,
        success: (res) => {
            try {

                if (res.data.isFinished) {
                    checkSocketProcess = true;
                    //$("#loaddiv").fadeOut();
                    buttonLoaderClose()
                    //loader kısmı icin burası 
                }
                console.log(res);
                OtoParcaView(res);

                //column arama ekleniyor
                if (checkSocketProcess == true) {
                    fnk_DataTable_Destroy();
                    fnk_DataTable_Init();
                }
            } catch (e) {

            }
        },
        error: (xhr, status) => { },
        complete: (xhr, status) => {
            setTimeout(function () {
                if (!checkSocketProcess) {
                    fnk_CheckOfferSorguSocket(guid);
                }
            }, 2500);
        }
    });

}



// OtoRobot Baglanılan Sirketler
function OtoRobotSirketler_View(data) {

    var res = window.location.pathname
    var excelsayfasimi = res.includes("IndexExcel")
    var exceltedarikciliste = [7, 41,68,92]  //toplusorguya gitmeyecek olan tedarikciler.
    var aciktedarikcisayisi = 0
    if (data != null)
        if (data.logins != null) {
            var tbodyDom = document.querySelector("#panelLoginSirket");
            var checklist = document.querySelectorAll("#flexSwitchCheckChecked")
            var oldlist = [], newlist = [];
            if (checklist.length == 0) {
                checklist = document.querySelectorAll("#flexSwitchCheckChecked1")
                checklist.forEach(function (i, j) {
                    oldlist.push(parseInt(i.getAttribute("value")))
                })
            }
            else {
                checklist.forEach(function (i, j) {
                    oldlist.push(parseInt(i.value))
                })
            }
            //console.log(oldlist);
            if (document.getElementById("tr_loading"))
                tbodyDom.removeChild(document.getElementById("tr_loading"))

            if (data.logins.length > 0) {
                var kapaliTedarikciler = [];
                for (var i = 0; i < data.logins.length; i++) {
                    var item = data.logins[i];
                    if (!item.isOpen) {
                        kapaliTedarikciler.push(item);
                        continue;
                    }
                    if (excelsayfasimi)
                        if (exceltedarikciliste.includes(item.sirketKodu))
                            continue;
                    aciktedarikcisayisi++;
                    newlist.push(item.sirketKodu)
                    if (!oldlist.includes(item.sirketKodu)) {
                        var inputs = ""
                        if (SorgulamaTuru == 1) {
                            inputs = `<td><div class="form-check form-switch"><input class="form-check-input" type="checkbox" role="switch" id="flexSwitchCheckChecked" checked value="${item.sirketKodu}" ></div></td>`;
                        }
                        var template = '<tr>' + inputs + '<td><a id="flexSwitchCheckChecked1" target="_blank" value="' + item.sirketKodu + '" href="' + item.siteUrl + '">' + item.sirketAdi + '</a></td>'
                        template += '<td>' + item.telefon + '</td></tr>'
                        $("#panelLoginSirket").append(template);
                    }
                }

                $("#panelClosedSirket").html("");
                for (var i = 0; i < kapaliTedarikciler.length; i++) {
                    var item = kapaliTedarikciler[i];

                    if (excelsayfasimi)
                        if (exceltedarikciliste.includes(item.sirketKodu))
                            continue;
                    var template = '<tr><td></td>'
                    template += '<td><a id="" target="_blank" value="' + item.sirketKodu + '" href="' + item.siteUrl + '">' + item.sirketAdi + '</a></td>'
                    template += '<td>' + item.telefon + '</td>'
                   /* template += '<td><img src="/assets/images/refresh.png" onclick="fnk_relogin('+item.sirketKodu+')"></td></tr>'*/


                       
                    $("#panelClosedSirket").append(template);
                }

                $("#span_kapali_sirket_sayisi").html("(" + kapaliTedarikciler.length + ")");

            }
            if (oldlist.length > 0) {
                oldlist.forEach(function (i, j) {
                    if (!newlist.includes(i)) {
                        //console.log(i)
                        var temp = document.querySelector('a[id="flexSwitchCheckChecked1"][value="' + i + '"]')
                        if (temp) {
                            temp = temp.parentNode.parentNode
                            tbodyDom.removeChild(temp)
                        }
                    }
                })
            }
            //tbodyDom.innerHTML = "";

            //data.logins.forEach((item) => {
            //    //console.log(item)
            //    //tbodyDom.innerHTML += `< li class="list-group-item" > <a target="_blank" href="${item.siteUrl}">${item.sirketAdi}</a> - <span>${item.telefon}</span></li > `
            //    tbodyDom.innerHTML += `
            //    <tr>
            //        <td>
            //            <div class="form-check form-switch">
            //                <input class="form-check-input" type="checkbox" role="switch"
            //                    id="flexSwitchCheckChecked" checked value="${item.sirketKodu}">
            //            </div>
            //        </td>
            //        <td><a target="_blank" href="${item.siteUrl}">${item.sirketAdi}</a></td>
            //        <td>${item.telefon}</td>
            //    </tr>`
            //})
            //$("#span_sirket_sayisi").html("(" + data.logins.length + ")")
            $("#span_sirket_sayisi").html("(" + aciktedarikcisayisi + ")")


        }
}
// Oto Parca Listesi View
function OtoParcaView(data) {
    var tableDom = document.querySelector("#sorguParcaList");
    //console.log(data);
    if (data.offerResult.length > 0) {
        fnk_DataTable_Destroy();
        tableDom.innerHTML = "";
        document.getElementById("SorguDetayRow").innerHTML = "";
        var sorguSonucDetay = "";
        var sorguOturumDetay = "";
        data.offerResult.forEach((item) => {

            console.log(item)
            if (item.sirketAd == "ÖZÇETE" || item.sirketAd == "DEGA") {
                if (document.getElementById("h4_" + item.sirketAd.toLowerCase()) == null) {
                    var h4 = document.createElement("h4");
                    h4.innerText = item.sirketAd;
                    h4.id = "h4_" + item.sirketAd.toLowerCase()
                    document.getElementById("div_img").appendChild(h4);

                    if (item.imageCheck) {
                        var tempimg = document.createElement("img");
                        tempimg.src = item.imageBase64;
                        tempimg.classList.add('img-fluid');
                        document.getElementById("div_img").appendChild(tempimg);
                    }
                    else {
                        var h4 = document.createElement("span");
                        h4.innerText = "Sonuç Bulunamadı.";
                        document.getElementById("div_img").appendChild(h4);

                    }
                }
            }
            else {



                if (item.yanitVarmi == 1) {

                    sorguSonucDetay += `
                                <div class="col-6 col-sm-4 col-lg-2 mb-3">
                                    <span class="fw-bold">${item.sirketAd}</span> <img src="/images/x-circle-fill.svg" alt="x-circle" />
                                </div>`
                }
                else if (item.yanitVarmi == 2) {
                    sorguOturumDetay += `
                                <div class="col-6 col-sm-4 col-lg-2 mb-3">
                                    <span class="fw-bold">${item.sirketAd}</span> <img src="/images/do-not-disturb-on.svg" alt="do-not" />
                                </div>`
                }
                else {
                    var kdvli = stringIsOrEmpty(item.kdvLi).replace(".", "").trim();
                    var kdvsiz = stringIsOrEmpty(item.kdvSiz).replace(".", "").trim();



                    if ((kdvli == null || kdvli == "") && (kdvsiz != null || kdvsiz != "")) {

                        var res = kdvsiz.replaceAll(".", "").replace(",", ".");
                        kdvli = (parseFloat(res) * 1.20).toFixed(2).replaceAll(".", ",").trim();

                        if (kdvli == "NaN") kdvli = "";

                    }
                    if ((kdvli != null || kdvli != "") && (kdvsiz == null || kdvsiz == "")) {

                        var res = kdvli.replaceAll(".", "").replace(",", ".");
                        kdvsiz = (parseFloat(res) / 1.20).toFixed(2).replaceAll(".", ",").trim();
                      
                        if (kdvsiz == "NaN") kdvsiz = "";

                    } 
                    tableDom.innerHTML += `
                                <tr>
                                    <td>
                                      <div style="float: left;">${item.sirketAd} |</div>
                                      ${UruneGit(item.sirketKodu, item.oemNo)}
                                      ${sepetYetki ? SepeteEkleButton(item.oemNo, item.stokNo, item.sirketKodu) : ""}
                                      ${item.sirketAd === "OtoDemir" ? '<div></br><a href="https://wa.me/902126713030" target="_blank" style="margin-left: 3px;"><img src="/images/wpotodemir.png" style="width: 30px; height: auto;"</a><a href="https://wa.me/902126713030 "target="_blank">İletişime Geç</a></div>' : ''}
                                    </td>
                                    <td>${stokDurumDesign(item.stok)}</td>
                                    <td style="word-break:break-word;">${stringIsOrEmpty(item.stokNo, item.sirketKodu)}</td>
                                    <td style="word-break:break-word;">${item.oemNo == null ? "" : item.oemNo}</td>
                                    <td>${item.urunAd.length > 20 ? item.urunAd.substr(0, 20) + `...<div class="tooltipExm">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24"
                                                viewBox="0 0 24 24">
                                                <path fill="#229"
                                                    d="M12 22c5.5 0 10-4.5 10-10S17.5 2 12 2S2 6.5 2 12s4.5 10 10 10M11 7h2v2h-2V7m3 10h-4v-2h1v-2h-1v-2h3v4h1v2Z" />
                                            </svg>
                                            <span class="tooltipExmtext">${item.urunAd}</span>
                                        </div>` : item.urunAd}</td>
                                     <td style="text-align: center">${item.marka}</td>
                                    <td style="text-align: center">${RenkveFiyatTemizle(kdvsiz)}</td>
                                    <td style="text-align: center">${RenkveFiyatTemizle(kdvli)}</td>
                                    <td style="text-align: center">${stringIsOrEmpty(item.listeFiyat).replace(".", "").trim() } </td>
                                    <td style="text-align: center">${tumFiyatDesign(item.tumFiyat) + MaliyetFiyati(kdvsiz) + MaliyetFiyati(kdvli) }</td>
                                </tr>`;
                    //tableDom.innerHTML += `
                    //            <tr>
                    //                <td><div style="float: left;">${item.sirketAd} |</div> ${UruneGit(item.sirketKodu, item.oemNo)}</td>
                    //                <td>${stokDurumDesign(item.stok)}</td>
                    //                <td style="word-break:break-word;">${stringIsOrEmpty(item.stokNo, item.sirketKodu)}</td>
                    //                <td style="word-break:break-word;">${item.oemNo == null ? "" : item.oemNo}</td>
                    //                <td>${item.urunAd.length > 20 ? item.urunAd.substr(0, 20) + `...<div class="tooltipExm">
                    //                        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24"
                    //                            viewBox="0 0 24 24">
                    //                            <path fill="#229"
                    //                                d="M12 22c5.5 0 10-4.5 10-10S17.5 2 12 2S2 6.5 2 12s4.5 10 10 10M11 7h2v2h-2V7m3 10h-4v-2h1v-2h-1v-2h3v4h1v2Z" />
                    //                        </svg>
                    //                        <span class="tooltipExmtext">${item.urunAd}</span>
                    //                    </div>` : item.urunAd}</td>
                    //                <td style="text-align: center">${item.marka}</td>
                    //                <td style="text-align: center">${stringIsOrEmpty(item.kdvSiz).replace(".", "").trim()}</td>
                    //                <td style="text-align: center">${stringIsOrEmpty(item.kdvLi).replace(".", "").trim()}</td>
                    //                <td style="text-align: center">${stringIsOrEmpty(item.listeFiyat).replace(".", "").trim()}</td>
                    //                <td style="text-align: center">${tumFiyatDesign(item.tumFiyat)}</td>
                    //            </tr>`;
                }

            }

        })

        if (sorguSonucDetay) {
            document.getElementById("SorguDetayRow").innerHTML += `
                                <div class="col-12 mt-3">
                                    <div class="card">
                                        <div class="card-header">
                                            <span class="fw-bold">Sonuç Yok</span> <img src="/images/x-circle-fill.svg" alt="x-circle" />
                                            <div class="tooltipExm">
                                                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24"
                                                    viewBox="0 0 24 24">
                                                    <path fill="#229"
                                                        d="M12 22c5.5 0 10-4.5 10-10S17.5 2 12 2S2 6.5 2 12s4.5 10 10 10M11 7h2v2h-2V7m3 10h-4v-2h1v-2h-1v-2h3v4h1v2Z" />
                                                </svg>
                                                <span class="tooltipExmtext">Aranan OEM numarası ile ilgili parça bulunamayan tedarikçiler.</span>
                                            </div>
                                        </div>
                                        <div class="card-body">
                                            <div class="row">
                                                ${sorguSonucDetay}
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>`
        }
        if (sorguOturumDetay) {
            document.getElementById("SorguDetayRow").innerHTML += `
                            <div class="col-12 mt-3">
                                <div class="card">
                                    <div class="card-header">
                                        <span class="fw-bold">Oturum Açılamadı</span> <img src="/images/do-not-disturb-on.svg" alt="do-not" />
                                        <div class="tooltipExm">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24"
                                                viewBox="0 0 24 24">
                                                <path fill="#229"
                                                    d="M12 22c5.5 0 10-4.5 10-10S17.5 2 12 2S2 6.5 2 12s4.5 10 10 10M11 7h2v2h-2V7m3 10h-4v-2h1v-2h-1v-2h3v4h1v2Z" />
                                            </svg>
                                            <span class="tooltipExmtext">Oturum açılamayan tedarikçiler.<br />Kullanıcı Kodu ve Şifreniz güncel mi? Kontrol ediniz.</span>
                                        </div>
                                    </div>
                                    <div class="card-body">
                                        <div class="row">
                                            ${sorguOturumDetay}
                                        </div>
                                    </div>
                                </div>
                            </div>`
        }
        SepeteEkleEvent();
        /*fnk_DataTable_Init();*/
    }
    else {
        var template = "";
        var sutunSayisi = $("#table_offer th")
        //sutunSayisi = 10;
        sutunSayisi = 9;
        for (var i = 0; i < sutunSayisi; i++) {
            template += `<td style="display: none;"></td>`
        }
        //template += `<td colspan="10" class="text-center">Sonuç Bulunamadı.</td>`;
        template += `<td colspan="10" class="text-center">Sonuç Bulunamadı.</td>`;
        tableDom.innerHTML = template;
    }
    //$('#table_offer').DataTable({ "pageLength": -1 })
    //fnk_DataTable_Init();

}
function tumFiyatDesign(fiyat) {
    // fiyat = ["listefiyati:123,2","kdvli:2132","kdvsiz:231,2"]; Ornek Format
    if (fiyat == undefined) return "";
    var fiyatinfo = "";

    fiyatinfo += `<table class="table table-dark table-bordered text-light m-0">`;
    for (var i = 0; i < fiyat.length; i++) {
        if (fiyat[i].includes(":")) {
            var baslikvefiyat = fiyat[i].split(":");
            fiyatinfo += `
                        <tr>
                            <td>${baslikvefiyat[0]}</td>
                            <td>${baslikvefiyat[1]}</td>
                        </tr>`;
        }
        else {
            fiyatinfo += `<tr><td colspan="2">${fiyat[i]}</td></tr>`;
        }
    }
    fiyatinfo += `</table>`;


    var fiyatText = `
                    <div class="tooltipExm1">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24">
                        <path fill="#229"
                            d="M12 22c5.5 0 10-4.5 10-10S17.5 2 12 2S2 6.5 2 12s4.5 10 10 10M11 7h2v2h-2V7m3 10h-4v-2h1v-2h-1v-2h3v4h1v2Z" />
                    </svg>
                    <div class="tooltipExmtext1">
                        ${fiyatinfo}
                    </div>
                </div>`;
    return fiyatText;
}
function stokDurumDesign(stok) {
    var tempStok = stok.replaceAll("Var", "<span class='text-success'>Var</span>").replaceAll("Yok", "<span class='text-danger'>Yok</span>")
    if (stok.length < 5) {
        tempStok += `
        <div class="tooltipExm">
            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24"
                viewBox="0 0 24 24">
                <path fill="#229"
                    d="M12 22c5.5 0 10-4.5 10-10S17.5 2 12 2S2 6.5 2 12s4.5 10 10 10M11 7h2v2h-2V7m3 10h-4v-2h1v-2h-1v-2h3v4h1v2Z" />
            </svg>
            <span class="tooltipExmtext">Merkez:${tempStok}</span>
        </div>`;
        return tempStok;
    }

    var textInfo = "<span class='text-danger'>Yok</span>";
    if (stok.includes("Var")) textInfo = "<span class='text-success'>Var</span>";

    textInfo += `
             <div class="tooltipExm">
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24"
                    viewBox="0 0 24 24">
                    <path fill="#229"
                        d="M12 22c5.5 0 10-4.5 10-10S17.5 2 12 2S2 6.5 2 12s4.5 10 10 10M11 7h2v2h-2V7m3 10h-4v-2h1v-2h-1v-2h3v4h1v2Z" />
                </svg>
                <span class="tooltipExmtext">${tempStok}</span>
            </div>`;
    return textInfo;
}
function stringIsOrEmpty(deger, sirketkod = 0) {

    if (deger == null) {
        return "";
    }
    //if (sirketkod == 27 || sirketkod == 32) {
    //    return MartasProductDeCrypt(deger);
    //}
    //else if (sirketkod == 80) {

    //    return RotasProductDeCrypt(deger)
    //}
    else if (sirketkod == 14) {
        return OzasProductDeCrypt(deger)
    }
    else if (sirketkod == 37) {
        return KayaparProductDeCrypt(deger)
    }
    else if (sirketkod == 65) return WinkelProductDeCrypto(deger)
    return deger;
}

function UruneGit(sirketKodu, oemnumarasi) {
    var result = "";

    switch (sirketKodu) {
        case 1:
            result = `
                    <form action="https://b2b.mercanlar.com/" method="post" target="_blank"> 
                      <input type="hidden" id="spare_part_number" name="spare_part_number"value="${oemnumarasi}">
                      <input type="hidden" id="databasename" name="databasename" value="MERCANLAR">
                      <input type="hidden" id="list_type" name="list_type" value="1">
                       <input type="hidden" id="isonlinestock" name="isonlinestock" value="False">
                        <input type="hidden" id="spare_part_brand" name="spare_part_brand" value="">
	                     <input type="hidden" id="spare_car_brand_name" name="spare_car_brand_name" value="">
                      <input type="submit" `+ urunGitButton() + ` >
                    </form>`;
            break;
        case 2:
            result = `<input type="button" onclick="window.open('https://www.b4bbasbug.com/Arama/UrunArama?TumAlanlarda=${oemnumarasi}');" ` + urunGitButton() + ` />`
            break;
        case 3:
            result = `<input type="button" onclick="window.open('https://b2b.artotomobil.com/products?ProductName=${oemnumarasi}');" ` + urunGitButton() + ` />`
            break;
        case 4:
            result = `<input type="button" onclick="window.open('https://b2b.universeparts.com/UrunArama/?Key=${oemnumarasi}');" ` + urunGitButton() + ` />`
            break;
        case 5:
            result = `<input type="button" onclick="window.open('https://b2b.otoismail.com.tr/Arama?id=${oemnumarasi}');" ` + urunGitButton() + ` />`
            break;
        case 6:
            result = `<input type="button" onclick="window.open('https://bayi.otokocparca.com/ParcaNoAramaSonucNew.aspx?partNumber=${oemnumarasi}&tip=1&AracMarka=&ParcaMarkaId=undefined');" ` + urunGitButton() + ` />`
            break;
        case 7:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://www.onlineparca.com/default.aspx');" ` + urunGitButton() + ` />`
            break;
        case 8:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://order.makroford.com/Stok');" ` + urunGitButton() + ` />`
            break;
        case 9:
            result = `<input type="button" onclick="copy('${oemnumarasi}','http://b2b.kamaotomotiv.com.tr/Stok');" ` + urunGitButton() + ` />`
            break;
        case 10:
            result = `<input type="button" onclick="window.open('https://demo.ikimizoto.com.tr/productsearch?key=${oemnumarasi}&showType=0');" ` + urunGitButton() + ` />`
            break;
        case 11:
            result = `<input type="button" onclick="window.open('https://bayi.hemeraotomotiv.com/productsearch?key=${oemnumarasi}&showType=0');" ` + urunGitButton() + ` />`
            break;
        case 12:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b4b.geneloto.com.tr/Search');" ` + urunGitButton() + ` />`
            break;
        case 13:
            result = `<input type="button" onclick="window.open('https://www.dpar.com.tr/searchproduct?PartNumber=${oemnumarasi}');" ` + urunGitButton() + ` />`
            break;
        case 14:
            result = `<input type="button" onclick="copy('${oemnumarasi}','http://program.ozas.com.tr/web/b2b/search');" ` + urunGitButton() + ` />`
            break;
        case 15:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.opar.com/Search');" ` + urunGitButton() + ` />`
            break;
        case 16:
            result = `<input type="button" onclick="window.open('https://bayi.phira.com.tr/productsearch?key=${oemnumarasi}&showType=0');" ` + urunGitButton() + ` />`
            break;
        case 17:
            result = `<input type="button" onclick="window.open('https://b2b.remarotomotiv.com/productsearch?key=${oemnumarasi}&showType=0');" ` + urunGitButton() + ` />`
            break;
        case 18:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://m.seneroto.com.tr/Stok');" ` + urunGitButton() + ` />`
            break;
        case 19:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://zirveotomotivb2b.com/bayi/stoklist.aspx');" ` + urunGitButton() + ` />`
            break;
        case 20:
            result = `<input type="button" onclick="copy('${oemnumarasi}','http://b2b.kaplanbayi.com/Stok');" ` + urunGitButton() + ` />`
            break;
        case 21:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://www.tireasotomotiv.com/anasayfa.asp');" ` + urunGitButton() + ` />`
            break;
        case 22:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://bayi.ozcete.com.tr/Products');" ` + urunGitButton() + ` />`
            break;
        case 23:
            result = `
                    <form action="https://voparotomotiv.com/Search/GetProductsWithPartNumber" method="post" target="_blank">
                      <input type="hidden" id="headerSearchType" name="headerSearchType"value="01">
                      <input type="hidden" id="PartNumber" name="PartNumber" value="${oemnumarasi}">
                      <input type="hidden" id="IsKdv" name="IsKdv" value="on">
                      <input type="submit" ` + urunGitButton() + ` ">
                    </form>`;
            break;
        case 24:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b4b.neko.com.tr/b4b/Search');" ` + urunGitButton() + ` />`
            break;
        case 25:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.bilesikplatform.com/stok');" ` + urunGitButton() + ` />`
            break;
        case 26:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.erdagotomotiv.com/Search.aspx');" ` + urunGitButton() + ` />`
            break;
        case 27:
            result = `<input type="button" onclick="copy('${oemnumarasi}','http://online.martas.com.tr/web/b2b/search');" ` + urunGitButton() + ` />`
            break;
        case 28:
            result = `<input type="button" onclick="window.open('https://b2b.merkezoto.com.tr/UrunArama/?Key=${oemnumarasi}');" ` + urunGitButton() + ` />`
            break;
        case 29:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.erkoto.com/Stok');" ` + urunGitButton() + ` />`
            break;
        case 30:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.cansunoto.com/Search.aspx');" ` + urunGitButton() + ` />`
            break;
        case 31:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.dogan-oto.com.tr/bayi/homedogan.aspx');" ` + urunGitButton() + ` />`
            break;
        case 32:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://bayi.adaoto.com.tr/web/b2b/search');" ` + urunGitButton() + ` />`
            break;
        case 33:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://www.ercammotorluaraclar.com.tr/default.aspx');" ` + urunGitButton() + ` />`
            break;
        case 34:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.babacandisticaret.com/Search.aspx');" ` + urunGitButton() + ` />`
            break;
        case 36:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b4b.himpeks.com.tr/Search');" ` + urunGitButton() + ` />`
            break;
        case 37:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.kayapar.com/web/b2b/search');" ` + urunGitButton() + ` />`
            break;
        case 38:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://satis.kilincotoyedekparca.com/Default.aspx');" ` + urunGitButton() + ` />`
            break;
        case 39:
            result = `<input type="button" onclick="window.open('https://b2b.ilkemotomotiv.com//UrunArama/?Key=${oemnumarasi}');" ` + urunGitButton() + ` />`
            break;
        case 40:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.parcabul.com.tr/Search.aspx'); " ` + urunGitButton() + `  />`
            break;
        case 42:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.karakasoto.com/Default.aspx'); " ` + urunGitButton() + `  />`
            break;
        case 43:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.nesotoparcalari.com/Search.aspx'); " ` + urunGitButton() + `  />`
            break;
        case 44:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b4b.motorasin.com/Search'); "` + urunGitButton() + `  />`
            break;
        case 45:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2bgulal.com/UrunArama'); "` + urunGitButton() + `  />`
            break;
        case 46:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.suleymanotomotiv.com.tr/Search.aspx'); "` + urunGitButton() + `  />`
            break;
        case 47:
            result = `<input type="button" onclick="window.open('https://b2b.asinotomotiv.com/search.aspx?q=${oemnumarasi}');" ` + urunGitButton() + ` />`
            break;
        case 48:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://online.basariotomotive.com/Search.aspx'); "` + urunGitButton() + `  />`
            break;
        case 49:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://online.bsdotomotiv.com/web/b2b/search'); "` + urunGitButton() + `  />`
            break;
        case 50:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b4b.hamlesiparis.com/Search'); "` + urunGitButton() + `  />`
            break;
        case 51:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.sayiner.com.tr/Search.aspx'); "` + urunGitButton() + `  />`
            break;
        case 52:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://shop.kazdal.com/'); "` + urunGitButton() + `  />`
            break;
        case 53:
            result = `<input type="button" onclick="window.open('https://b2b.basaran-otomotiv.com.tr/search.aspx?q=${oemnumarasi}');" ` + urunGitButton() + ` />`

            break;
        case 54:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.koreaotomotiv.com/Search.aspx'); "` + urunGitButton() + `  />`
            break;
        case 55:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.askarauto.com/Search.aspx'); "` + urunGitButton() + `  />`
            break;
        case 56:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://web.lepar.com.tr/Search.aspx'); "` + urunGitButton() + `  />`
            break;
        case 57:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.teknikdizel.com/Search.aspx'); "` + urunGitButton() + `  />`
            break;
        case 58:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b4b.gungorotomotiv.net/Search'); "` + urunGitButton() + `  />`
            break;
        case 59:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.aktifotomotiv-hsm.com/Search.aspx'); "` + urunGitButton() + `  />`
            break;
        case 60:
            result = `<input type="button" onclick="window.open('https://b4b.altayoto.com/Search?text=${oemnumarasi}');" ` + urunGitButton() + ` />`
            break;

        case 61:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.alternatifotomotiv.com/Search.aspx'); "` + urunGitButton() + `  />`
            break;
        case 62:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.mtzotomotiv.com.tr/Search.aspx'); "` + urunGitButton() + `  />`
            break;
        case 63:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.oylumoto.com/Search.aspx'); "` + urunGitButton() + `  />`
            break;
        case 64:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.davetotomotiv.com.tr/Stok'); "` + urunGitButton() + `  />`
            break;
        case 65:
            result = `<input type="button" onclick="copy('${oemnumarasi}','http://online.winkelfilter.com/web/b2b/search'); "` + urunGitButton() + `  />`
            break;
        case 66:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://www.omotomotiv.com/Home'); "` + urunGitButton() + `  />`
            break;
        case 67:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.erturkmotorluaraclar.com/Search.aspx'); "` + urunGitButton() + `  />`
            break;
        case 68:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://www.parcahane.com/default.aspx'); "` + urunGitButton() + `  />`
            break;
        case 69:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://opss.borusanotomotiv.com/Purchase/ItemQuery'); "` + urunGitButton() + `  />`
            break;
        case 70:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.teknoparotomotiv.com/UrunArama'); "` + urunGitButton() + `  />`
            break;
        case 71:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.merkezotomotiv.com.tr/Search.aspx'); "` + urunGitButton() + `  />`
            break;
        case 72:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.ilerioto.com.tr/UrunArama'); "` + urunGitButton() + `  />`
            break;
        case 73:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b4b.megatekotomotiv.com/Search'); "` + urunGitButton() + `  />`
            break;
        case 74:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.duzgunotomotiv.com/UrunArama'); "` + urunGitButton() + `  />`
            break;
        case 75:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b4b.merkezoto.com.tr/UrunArama'); "` + urunGitButton() + `  />`
            break;
        case 76:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.arsanmotor.com.tr/UrunArama'); "` + urunGitButton() + `  />`
            break;
        case 77:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.karpartotomotiv.com.tr/UrunArama'); "` + urunGitButton() + `  />`
            break;
        case 78:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://iotob2b.intermobil.com.tr/Search.aspx'); "` + urunGitButton() + `  />`
            break;
        case 79:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.yenialkaotomotiv.com/Search.aspx'); "` + urunGitButton() + `  />`
            break;
        case 80:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.rotas.com.tr/web/b2b/search');" ` + urunGitButton() + ` />`
            break;
        case 81:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.balkanotoradyator.com/Search.aspx');" ` + urunGitButton() + ` />`
            break;
        case 82:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.mepayedekparca.com/UrunArama'); "` + urunGitButton() + `  />`
            break;
        case 83:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.asautoparts.com.tr/Search.aspx'); "` + urunGitButton() + `  />`
            break;
        case 84:
            result = `<input type="button" onclick="copy('${oemnumarasi}',' https://tekyildizb2b.boryaz.com/musteri/stok_arama.do?route=musteri/stok_arama'); "` + urunGitButton() + `  />`
            break;
        case 85:
            result = `<input type="button" onclick="window.open(' https://bayi.canli.com.tr/productsearch?key=${oemnumarasi}&showType=0');" ` + urunGitButton() + ` />`
            break;
        case 86:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.oznuroto.com.tr/Stok'); "` + urunGitButton() + `  />`
            break;
        case 87:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://bayi.tokotomarket.com.tr/Search.aspx'); " ` + urunGitButton() + `  />`
            break;
        case 88:
            result = `
                    <form action="https://b2b.votek.com.tr/Search/GetProductsWithPartNumber" method="post" target="_blank">
                      <input type="hidden" id="headerSearchType" name="headerSearchType"value="01">
                      <input type="hidden" id="PartNumber" name="PartNumber" value="${oemnumarasi}">
                      <input type="hidden" id="IsKdv" name="IsKdv" value="on">
                      <input type="submit" ` + urunGitButton() + ` ">
                    </form>`;
            break;
        case 91:
            result = `<input type="button" onclick="window.open('https://b4b.genckaya.com/index.php?search=${oemnumarasi}&formsent=1&sayfa=arama');" ` + urunGitButton() + ` />`
            break;
        case 92:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.kaymazlarmotor.com/Search.aspx'); " ` + urunGitButton() + `  />`
            break;
        case 93:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.gaysanotomotiv.com/Search.aspx'); " ` + urunGitButton() + `  />`
            break;
        case 94:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.asenotomotiv.com/Search.aspx'); " ` + urunGitButton() + `  />`
            break;
        case 95:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b4b.skopar.com/Search/Index'); " ` + urunGitButton() + `  />`
            break;
        case 96:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.gercekithalat.com/Search.aspx'); " ` + urunGitButton() + `  />`
            break;
        case 102:
            result = `<input type="button" onclick="window.open(' https://www.devliotomotiv.com/B2C/Search?Type=1&searchText=${oemnumarasi}');" ` + urunGitButton() + ` />`
            break;
        case 103:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.obis.com.tr/Search.aspx'); " ` + urunGitButton() + `  />`
            break;
        case 104:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.akyoloto.com/Search.aspx'); " ` + urunGitButton() + `  />`
            break;
        case 105:
            result = `<input type="button" onclick="window.open('https://b2b.goral.com.tr/UrunArama/?Key=${oemnumarasi}');" ` + urunGitButton() + ` />`
            break;
        case 110:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b4b.cansunoto.com/Search/Index'); " ` + urunGitButton() + `  />`
            break;

        case 121:
            result = `<input type="button" onclick="copy('${oemnumarasi}','http://program.akkaya.com.tr/web/b2b/search');" ` + urunGitButton() + ` />`
            break;

        case 122:
            result = `<input type="button" onclick="copy('${oemnumarasi}','http://program.bursanurpar.com.tr/web/b2b/search');" ` + urunGitButton() + ` />`
            break;
        case 125:
            result = `<input type="button" onclick="copy('${oemnumarasi}','http://program.somotomotiv.com.tr/web/b2b/search');" ` + urunGitButton() + ` />`
            break;
        case 140:
            result = `<input type="button" onclick="copy('${oemnumarasi}','https://b2b.himkaotomotiv.com/web/b2b/search');" ` + urunGitButton() + ` />`
            break;

        default:
    }
    var template = '<div style="float: left;">'
    template += result;
    template += '</div>'
    return template;
}
function RenkveFiyatTemizle(fiyat) {

    var result = "";
    //FATURASIZ KIRMIZI FİYAT  KODLARI START

    if (fiyat.includes("red")) {
        result = '<p style="color:red;">' + fiyat.replace("red", "") + '</p>'
    }
    else { result = '<p>' + fiyat + '</p>' }

    //FATURASIZ KIRMIZI FİYAT  KODLARI END

    //MALİYET FİYATI OLANLARDA MALİYET FİYATI BUTON KODLARI START

    if (fiyat.includes("maliyetfiyat")) {
        fiyat = fiyat.replace("maliyetfiyat", "")
        var fiyatinfo = "";
        var fiyatText = `
         <p>${fiyat}</p>
`;
        result = fiyatText;

    }


    //MALİYET FİYATI OLANLARDA MALİYET FİYATI BUTON KODLARI END
    return result;
}
function MaliyetFiyati(fiyat) {

    var result = "";


    //MALİYET FİYATI OLANLARDA MALİYET FİYATI BUTON KODLARI START

    if (fiyat.includes("maliyetfiyat")) {
        fiyat = fiyat.replace("maliyetfiyat", "")
        var fiyatinfo = "";

        fiyatinfo += `<table class="table table-dark table-bordered text-light m-0">`;



        fiyatinfo += `<tr><td stlye='width:auto;'>Maliyet Fiyatıdır. </td></tr>`;


        fiyatinfo += `</table>`;


        var fiyatText = `
                    <div class="tooltipExm1" >

                    <svg width="24px" height="24px" viewBox="0 0 24.00 24.00" xmlns="http://www.w3.org/2000/svg" fill="#a36262" stroke="#a36262" transform="matrix(1, 0, 0, 1, 0, 0)"><g id="SVGRepo_bgCarrier" stroke-width="0"></g><g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round" stroke="#CCCCCC" stroke-width="0.096"></g><g id="SVGRepo_iconCarrier"> <title></title> <g id="Complete"> <g id="alert-circle"> <g> <line fill="none" stroke="#ff8800" stroke-linecap="round" stroke-linejoin="round" stroke-width="2.4" x1="12" x2="12" y1="8" y2="12"></line> <line fill="none" stroke="#ff8800" stroke-linecap="round" stroke-linejoin="round" stroke-width="2.4" x1="12" x2="12" y1="16" y2="16"></line> <circle cx="12" cy="12" data-name="--Circle" fill="none" id="_--Circle" r="10" stroke="#ff8800" stroke-linecap="round" stroke-linejoin="round" stroke-width="2.4"></circle> </g> </g> </g> </g></svg>
                    <div class="tooltipExmtext1" style="width:150px;">
                        ${fiyatinfo}
                    </div>
                </div>`;



        result = fiyatText;

    }


    //MALİYET FİYATI OLANLARDA MALİYET FİYATI BUTON KODLARI END
    return result;
}



function urunGitButton() {
    return ' value="" style="background-image: url(\'/images/goBlue.png\');background-repeat: no-repeat;background-position-x: center;    background-position-y: center;    width: 45px;    border-style: none;    height: 29px;"'
}
function copy(oemnumararasi, url) {
    let elementText = oemnumararasi;

    let inputElement = document.createElement('input');
    inputElement.setAttribute('value', elementText);
    document.body.appendChild(inputElement);
    inputElement.select();
    document.execCommand('copy');
    inputElement.parentNode.removeChild(inputElement);

    window.open(url);

}
function OzasProductDeCrypt(J) {

    const P = k("hxt!236");
    return decodeURI(G(P(J)))
}
const k = J => {
    const P = D => (D => D.split("").map(V => V.charCodeAt(0)))(J).reduce((V, le) => V ^ le, D);
    return D => D.match(/.{1,2}/g).map(V => parseInt(V, 16)).map(P).map(V => String.fromCharCode(V)).join("")
}
    , G = J => {
        for (var I = J.toString(), P = "", D = 0; D < I.length; D += 2)
            P += String.fromCharCode(parseInt(I.substr(D, 2), 16));
        return P
    }

function MartasProductDeCrypt(be) {

    const b = I("hxt!236");
    return decodeURI(A(b(be)))
}
const I = be => {
    const b = T => (T => T.split("").map(k => k.charCodeAt(0)))(be).reduce((k, q) => k ^ q, T);
    return T => T.match(/.{1,2}/g).map(k => parseInt(k, 16)).map(b).map(k => String.fromCharCode(k)).join("")
}
    , A = be => {
        for (var U = be.toString(), b = "", T = 0; T < U.length; T += 2)
            b += String.fromCharCode(parseInt(U.substr(T, 2), 16));
        return b
    }

//function RotasProductDeCrypt(be) {

//    const M = btoa("7480a729-6468-4957-97c9-3b96adc0aeff");
//    var qwe = decodeURIComponent(atob(be.replace(M, "")));

//    return qwe

//}


function KayaparProductDeCrypt(ge) {

    const y = btoa("7480a729-6468-4957-97c9-3b96adc0aeff");
    var qwe = decodeURIComponent(atob(ge.replace(y, "")));

    return qwe

}

function WinkelProductDeCrypto(t) {
    if (!t || "string" != typeof t || t.length < 49)
        return t;
    try {
        return decodeURIComponent(atob(t.replace(o, "")))
    } catch (e) {
        return t
    }
}


function SepeteEkleEvent() {

    $('.btn_sepete_ekle').unbind('click');

    $(".btn_sepete_ekle").click((e) => {

        Swal.fire({
            title: 'Ürün Sepete Eklensin mi?',
            showCancelButton: true,
            confirmButtonText: 'İptal',
            confirmButtonColor: 'gray',
            cancelButtonText: 'Tamam',
            cancelButtonColor: "#0d6efd",
            focusCancel: true
        }).then((result) => {
            if (!result.isConfirmed) {
                var oem = e.target.getAttribute("value")
                var kod = e.target.getAttribute("value2")
                kod = parseInt(kod);
                var data = { "OemNumarasi": oem, "SorgulanacakFirmalar": [kod] };

                $.ajax({
                    type: "POST",
                    url: "/api/request/sendorder",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(data),
                    success: (res) => {
                        //console.log(res)
                        if (res.isFinished) {
                            MessageBox("Ürün Sepete Eklendi.")
                            var el = e.target
                            el.style.backgroundImage = 'url("/images/checkout3.png")';
                        }
                        else {
                            MessageBox("Ürün Sepete Eklenemedi.\nLütfen daha sonra tekrar deneyiniz.");
                        }
                    },
                    error: (xhr, status) => { },
                    complete: (xhr, status) => {

                    }
                });
            }
        });
    })
}

function SepeteEkleButton(oem, stokno, kod) {
    var aktifTedarikci = [1, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 16, 17, 18, 19, 20, 23, 24, 30, 31, 33, 34, 36, 39, 40, 42, 43, 44, 47, 48, 50, 53, 54, 55, 56, 57, 58,/* 59,*/ 60, 61, 62, 63, 64, 66, 67, 68, 69, 71, 72, 73, 74, 75, 76, 78, 79, 80,81, 82, 84, 85, 86, 88, 90, 91, 92, 93, 94, 95, 96, 102, 103, 104, 105]
    if (!aktifTedarikci.includes(kod)) return "";
    if (oem != null)
        if (oem.trim() == "")
            oem = stokno
    if (oem == null) {
        oem = stokno
    }
    if (kod == 44 || kod == 84 || kod == 71 || kod == 73 || kod == 67 || kod == 53 || kod == 91 || kod == 92) { //motorasin stokno'dan sepete ekleme işlemi.
        if (stokno != null) {
            oem = stokno
        }

    }
    var template = '<a type="button" class="btn_sepete_ekle" value="' + oem + '" value2="' + kod + '" style="background-image: url(\'/images/checkout4.png\');background-repeat: no-repeat;background-position-x: center;background-position-y: center;width: 45px;border-style: none;height: 36px;background-color: rgba(255, 255, 255, 0);"></a>'

    return template;
}
function MessageBox(mesaj) {
    Swal.fire({
        title: mesaj,
        confirmButtonText: 'Tamam',
    }).then((result) => {

    });
}