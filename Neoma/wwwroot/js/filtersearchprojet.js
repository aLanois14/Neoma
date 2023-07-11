function HighlightSearch() {
    var chaineRecherche = $("#searchText").val();
    chaineRecherche = chaineRecherche ? chaineRecherche.toLowerCase() : chaineRecherche;

    if (chaineRecherche.length >= 1) {
        var candidats = $(".item .w-100");
        var nomCandidat;
        var initialText;
        for (var i = 0; i < candidats.length; i++) {
            for (var j = 0; j < candidats[i].children.length; j++) {
                if (candidats[i].children[j].className === 'item-secondary') {
                    for (var k = 0; k < candidats[i].children[j].children.length; k++) {
                        if (candidats[i].children[j].children[k].nodeName === 'H3') {
                            nomCandidat = candidats[i].children[j].children[k].innerHTML;
                            candidats[i].children[j].children[k].innerHTML = ReplaceText(chaineRecherche, nomCandidat);
                        }
                        else if (candidats[i].children[j].children[k].nodeName === 'P') {
                            nomCandidat = candidats[i].children[j].children[k].innerHTML;
                            candidats[i].children[j].children[k].innerHTML = ReplaceText(chaineRecherche, nomCandidat);
                        }
                        else if (candidats[i].children[j].children[k].nodeName === 'I') {
                            nomCandidat = candidats[i].children[j].children[k].children.innerHTML;
                            candidats[i].children[j].children[k].children.innerHTML = ReplaceText(chaineRecherche, nomCandidat);
                        }
                    }
                }
                else if (candidats[i].children[j].className === 'item-lines') {
                    for (var n = 0; n < candidats[i].children[j].children.length; n++) {
                        //if (candidats[i].children[j].children[n].nodeName === 'UL') {
                        for (var m = 0; m < candidats[i].children[j].children[n].children.length; m++) {
                            if (candidats[i].children[j].children[n].children[m].className !== 'item-col item-col-role entete' && candidats[i].children[j].children[n].children[m].className !== 'item-col item-col-specialty entete' && candidats[i].children[j].children[n].children[m].className !== 'item-col item-col-user') {
                                if (candidats[i].children[j].children[n].children[m].className.trim() === 'item-col item-col-role') {
                                    initialText = candidats[i].children[j].children[n].children[m].innerHTML;
                                    candidats[i].children[j].children[n].children[m].innerHTML = ReplaceText(chaineRecherche, initialText);
                                }    
                                else if (candidats[i].children[j].children[n].children[m].className.trim() === 'item-col item-col-specialty') {
                                    initialText = candidats[i].children[j].children[n].children[m].children[0].innerHTML;
                                    candidats[i].children[j].children[n].children[m].children[0].innerHTML = ReplaceText(chaineRecherche, initialText);
                                }    
                            }
                        }
                    }
                }
            }
        }
    }
}

function ReplaceText(chaineRecherche, initialText) {
    var offset = 0;
    var position = 0;
    var textReplace = initialText;
    initialText = initialText ? initialText.toLowerCase() : initialText;

    while (position !== -1) {
        position = initialText.indexOf(chaineRecherche);
        if (position !== -1) {
            textReplace = textReplace.substr(0, position + offset + chaineRecherche.length) + '</span>' + textReplace.substr(position + offset + chaineRecherche.length);
            textReplace = textReplace.substr(0, position + offset) + '<span style="background-color:gold">' + textReplace.substr(position + offset);
            initialText = initialText.substr(position + chaineRecherche.length);
            offset += position + chaineRecherche.length + '</span><span style="background-color:gold">'.length;
        }
    }

    return textReplace;
}