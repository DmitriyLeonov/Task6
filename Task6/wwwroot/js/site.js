
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
                        if (v.IsRead == true) {
                            tr += `
                            <tr class="">
                                <td class="view-message dont-show">${v.Sender}</td>
                                <td class="view-message"><a href="/Messages/Details/${v.Id}"$>${v.Title} </a></td>
                                <td class="view-message inbox-small-cells"></td>
                                <td class="view-message text-right">${v.Created}</td>
                            </tr>
                            `
                        } else {
                            tr += `
                            <tr class="unread">
                                <td class="view-message dont-show">${v.Sender}</td>
                                <td class="view-message"><a href="/Messages/Details/${v.Id}"$>${v.Title} </a></td>
                                <td class="view-message inbox-small-cells"></td>
                                <td class="view-message text-right">${v.Created}</td>
                            </tr>
                            `
                        }
                        //tr += ` <tr>  
                        //            <td> ${v.Title}</td>
                        //            <td> ${v.Body}</td>
                        //            <td> ${v.Sender}</td>
                        //            <td>${v.Created}</td>
                        //        </tr>`
                    })

                $("#tblInfo").html(tr);
            },
            error:(error) => {
                console.log(error);
            }
        });
    }
})