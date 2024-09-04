# PortfolioManager

> [!Note]
> This project was initially made for a very spcific page but shall now be going into a more general direction.

> [!Important]
> For now, there is a specific structure needed in the folders and html for this to work.


### Needed in HTML
Illustration container: (Images get added and removed here)
```
<div id="IllustrationContainer"></div>
```
Filter buttons: (all is required, the rest will be generated depending on the tag settings)
```
<div class="filter_buttons">
       <button data-filter="all">ALL</button>
</div>
```
(Optional) Individual headings:
```
<h3 class="filterHeading"></h3>
```

### Needed JavaScript files:
If you chose to not use individual headings, just remove the red marked parts.
```diff
document.addEventListener("DOMContentLoaded", function () {
  const buttons = document.querySelectorAll(".filter_buttons button");
  const items = document.querySelectorAll("#IllustrationContainer .item");
-  const filterHeading = document.querySelectorAll(".filterHeading");

-  filterHeading.forEach((h) => {
-    h.style.display = "none";
-  });
  buttons.forEach((button) => {
    button.addEventListener("click", () => {
      const filter = button.getAttribute("data-filter");
      buttons.forEach((btn) => btn.classList.remove("active"));
      button.classList.add("active");

      items.forEach((item) => {
        if (filter === "all") {
          item.style.display = "flex";
        } else {
          item.style.display = item.classList.contains(filter)
            ? "flex"
            : "none";
        }
      });

-      filterHeading.forEach((h) => {
-        if (filter === "all") {
-          h.style.display = "none";
-        } else {
-          h.style.display = h.classList.contains(filter) ? "flex" : "none";
-        }
-      });
    });
  });

  document.querySelector('[data-filter="all"]').click();
});

```

> [!Note]
> ### FYI
> Illustrations will have id "ill-1; ill-2" and so on. <br>
> Illustrations will always have class "item" + any amount of selected tags as class for sorting.
