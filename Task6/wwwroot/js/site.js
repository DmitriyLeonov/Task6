
$(() => {
    let token = "43jvnfdjf5mkcsn";
    var connection = new signalR.HubConnectionBuilder().withUrl("MessageHub", { accessTokenFactory: () => token }).build();
    connection.start();
    connection.on("LoadMessages", function() {
        LoadMessages()
    })

    LoadMessages();

    function LoadMessages(parameters) {
        var tr = '';

        $.ajax({
            url: "/Messages/GetMessages",
            method: 'GET',
            success: (result) => {
                
                $.each(result,
                    (k, v) => {
                        console.log(v);
                        tr += ` <tr>  
                                    <td> ${v.Id}</td>
                                    <td> ${v.Title}</td>
                                    <td> ${v.Body}</td>
                                    <td> ${v.Sender}</td>
                                    <td>${v.Created}</td>
                                    <td> ${v.Reciever}</td>
                                </tr>`
                    })

                $("#tblInfo").html(tr);
            },
            error:(error) => {
                console.log(error);
            }
        });
    }
})