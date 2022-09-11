

$('.modal-confirm').on('show.bs.modal', function (event) {

    var button = $(event.relatedTarget); // Button that triggered the modal
    var formToSubmit = button.data('confirm-form'); // Extract info from data-* attributes
    var title = button.data('modal-title');
    var message = button.data('modal-message');

    var modal = $(this);
    modal.find('.modal-title').text(title);
    modal.find('.modal-body p').text(message);

    modal.find('button[data-action]').click(function () {
        $(formToSubmit).submit();
    });
})