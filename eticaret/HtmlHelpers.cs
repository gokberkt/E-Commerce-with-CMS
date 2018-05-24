using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eticaret
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString DeleteModal(this HtmlHelper htmlHelper, string url, string text, string title)
        {
            string html = "<div class='modal fade bs-example-modal-sm in' id='GeneralDeleteModal' tabindex='-1' role='dialog' aria-labelledby='GeneralDeleteModal' aria-hidden='true' style='display: none;'><div class='modal-dialog modal-sm'><div class='modal-content'><div class='modal-header'><button type = 'button' class='close' data-dismiss='modal' aria-hidden='true'>×</button><h4 class='modal-title' data-role='ModalTitle'>" + title + "</h4></div><div class='modal-body' data-role='ModalContent'>" + text + "</div><div class='modal-footer' data-role='ModalFooter'><button style='margin-right:10px;' type='button' class='btn btn-default' data-dismiss='modal' aria-hidden='true'>Kapat</button><a id='deletes' href='" + url + "' class='btn btn-danger'>Sil</a></div> </div></div></div>"
                + "<script type='text/javascript'> $('.deleteButton').click(function (){var Id = $(this).data('id');$('#deletes').attr('href', '"+url+ "?ID=' + Id + '');});</script>";

            return new MvcHtmlString(html);
        }
    }
}