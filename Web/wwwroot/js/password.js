const togglePasswordVisibility = () => {
  $(".input-group-text").each(function () {
    $(this).on("click", function () {
      const toggleId = $(this).data("toggle-id");
      const $input = $(`.password-field[data-toggle-id="${toggleId}"]`);
      const $icon = $(this).find("i");

      if ($input.attr("type") === "password") {
        $input.attr("type", "text");
        $icon.removeClass("fa-eye").addClass("fa-eye-slash");
      } else {
        $input.attr("type", "password");
        $icon.removeClass("fa-eye-slash").addClass("fa-eye");
      }
    });
  });
};

togglePasswordVisibility();
