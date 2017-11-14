var ip = "http://localhost:8084";

$(document).ready(function(){
    httpGetMusicsAsync();
    
    $("#buttonSave").click(function(){
    httpPostVoteAsync();
        /*var i =$('input[name=nome]:checked').val();
        alert(i);
        $.post( "http://httpbin.org/ip", { id: i } );*/
    });
});

function httpPostVoteAsync()
{
    var msg = false;
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.onreadystatechange = function() { 
        if (xmlHttp.status == 200) {
            var response = xmlHttp.responseText;
            var obj = $.parseJSON(response);
            if (obj.voted && !msg) {
                alert('Você já votou nesta lista.\nAguarde próximas músicas');
                msg = true;
            }
        }
    }
    xmlHttp.open("POST", ip + "/vote", true); // true for asynchronous 
    xmlHttp.setRequestHeader("Content-Type", "application/json; charset=UTF-8");
    
    if (document.getElementById('input0').checked) {
        xmlHttp.send(document.getElementById('input0').value);
    }else if (document.getElementById('input1').checked) {
        xmlHttp.send(document.getElementById('input1').value);
    }else if (document.getElementById('input2').checked) {
        xmlHttp.send(document.getElementById('input2').value);
    }else if (document.getElementById('input3').checked) {
        xmlHttp.send(document.getElementById('input3').value);
    }
}

function httpGetMusicsAsync()
{
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.onreadystatechange = function() { 
        if (xmlHttp.readyState == 4 && xmlHttp.status == 200)
            callback(xmlHttp.responseText);
    }
    //TODO: change this ip according to network
    xmlHttp.open("GET", ip + "/musics", true); // true for asynchronous 
    xmlHttp.send(null);
}

function callback(response) {
    var obj = $.parseJSON(response);
    
    document.getElementById('input0').value = obj[0].url;
    $("#musica1").append(obj[0].name); 
    $("#autor1").append(obj[0].artist);
    //$("#votos1").append(obj[0].url);

    document.getElementById('input1').value = obj[1].url;
    $("#musica2").append(obj[1].name);
    $("#autor2").append(obj[1].artist);
    //$("#votos2").append(obj[1].url);

    document.getElementById('input2').value = obj[2].url;
    $("#musica3").append(obj[2].name);
    $("#autor3").append(obj[2].artist);
    //$("#votos3").append(obj[2].url);

    document.getElementById('input3').value = obj[3].url;
    $("#musica4").append(obj[3].name);
    $("#autor4").append(obj[3].artist);
    //$("#votos4").append(obj[3].url);
}
/**/
