$(document).ready(function () {
  $.validator.setDefaults({
    highlight: function (element) {
      if ($(element).is(":radio") || $(element).is(":checkbox")) {
        var name = $(element).attr("name");
        $('input[name="' + name + '"]').addClass("is-invalid");
      } else {
        $(element).addClass("is-invalid");
      }
    },
    unhighlight: function (element) {
      if ($(element).is(":radio") || $(element).is(":checkbox")) {
        var name = $(element).attr("name");
        $('input[name="' + name + '"]').removeClass("is-invalid");
      } else {
        $(element).removeClass("is-invalid");
      }
    },
    errorElement: "div",
    errorClass: "invalid-feedback",
    errorPlacement: function (error, element) {
      var container = element.next(".text-danger");
      if (container.length) {
        container.html(error.html());
      } else {
        error.insertAfter(element);
      }
    },
  });
  $("form").each(function () {
    $(this).removeData("validator");
    $(this).removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse(this);
  });
});
$(document).ready(function () {
  $("form").on("invalid-form.validate", function () {
    $(this)
      .find(".input-validation-error")
      .each(function () {
        if ($(this).is(":radio") || $(this).is(":checkbox")) {
          var name = $(this).attr("name");
          $('input[name="' + name + '"]').addClass("is-invalid");
        } else {
          $(this).addClass("is-invalid");
        }
      });
  });
  $("form").on("valid-form.validate", function () {
    $(this)
      .find(".input-validation-valid")
      .each(function () {
        if ($(this).is(":radio") || $(this).is(":checkbox")) {
          var name = $(this).attr("name");
          $('input[name="' + name + '"]').removeClass("is-invalid");
        } else {
          $(this).removeClass("is-invalid");
        }
      });
  });
});
