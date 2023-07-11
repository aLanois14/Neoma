function HighlightSearch() {
    var chaineRecherche = $("#searchText").val();
    chaineRecherche = chaineRecherche ? chaineRecherche.toLowerCase() : chaineRecherche;

    if (chaineRecherche.length >= 1) {
        var candidats = $(".item-primary");
        var initialText;
        for (var i = 0; i < candidats.length; i++) {
            for (var j = 0; j < candidats[i].children.length; j++) {

                if (candidats[i].children[j].nodeName === 'H3') {

                    var nomCandidat = candidats[i].children[j].innerHTML;
                    candidats[i].children[j].innerHTML = ReplaceText(chaineRecherche, nomCandidat);
                }
                else if (candidats[i].children[j].nodeName === 'UL') {
                    for (var k = 0; k < candidats[i].children[j].children.length; k++) {

                        initialText = candidats[i].children[j].children[k].innerHTML;
                        candidats[i].children[j].children[k].innerHTML = ReplaceText(chaineRecherche, initialText);
                    }
                }
            }
        }

        candidats = $(".item-secondary");

        for (var p = 0; p < candidats.length; p++) {
            for (var q = 0; q < candidats[p].children.length; q++) {
                if (candidats[p].children[q].nodeName === 'UL') {
                    for (var o = 0; o < candidats[p].children[q].children.length; o++) {

                        initialText = candidats[p].children[q].children[o].innerHTML;
                        candidats[p].children[q].children[o].innerHTML = ReplaceText(chaineRecherche, initialText);
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