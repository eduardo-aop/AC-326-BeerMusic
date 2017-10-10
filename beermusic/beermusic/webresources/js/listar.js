$(document).ready(function(){
    
    $.get( "http://putsreq.com/6XrPhN8s54vw440e27Rv", function( data ) {
      alert(data);
      var jsonData = JSON.parse(data);
      alert("passou aqui");
      for (var i = 0; i < jsonData.musicas.length; i++) {
          var counter = jsonData.musicas[i];
          alert(counter.musica);
      }
      $("#musica1").append(data[0].musica); 
      $("#autor1").append(data[0].autor);
      $("#votos1").append(data[0].votos);

      $("#musica2").append(data[1].musica);
      $("#autor2").append(data[1].autor);
      $("#votos2").append(data[1].votos);

      $("#musica3").append(data[2].musica);
      $("#autor3").append(data[2].autor);
      $("#votos3").append(data[2].votos);

      $("#musica4").append(data[3].musica);
      $("#autor4").append(data[3].autor);
      $("#votos4").append(data[3].votos);
    });

    /*
    LISTA ESTASTICA
    $("#musica1").append("musica"); 
    $("#autor1").append("autor");
    $("#votos1").append("votos");

    $("#musica2").append("musica2");
    $("#autor2").append("autor2");
    $("#votos2").append("votos2");

    $("#musica3").append("musica3");
    $("#autor3").append("autor3");
    $("#votos3").append("votos3");

    $("#musica4").append("musica4");
    $("#autor4").append("autor4");
    $("#votos4").append("votos4");
*/
    $("#buttonSave").click(function(){
        var i =$('input[name=nome]:checked').val();
        alert(i);
        $.post( "http://httpbin.org/ip", { id: i } );
    });
});
/**/
