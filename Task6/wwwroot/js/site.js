import { signalR } from "../microsoft/signalr/dist/browser/"

$(() => {

    var connection = new signalR.HubConnectionBuilder().withUrl("/MessageHub").build();
    connection.start();
    connection.on("LoadMessages", function() {
        LoadMessages()
    })

    LoadMessages();

    function LoandMessages(parameters) {
        var tr = '';

        $.ajax({
            url: "/Messages/GetMessages",
            method: 'GET',
            success: (result) => {
                $.each(result,
                    (k, v) => {
                        tr += ` <tr>  
                                < td > ${v.Title}</td >
                                < td > ${v.Body}</td >
                                < td >${v.Created}</td><
                            /tr>`
                    })

                $("#tblInfo").html(tr);
            },
            error:(error) => {
                console.log(error);
            }
        });
    }
})