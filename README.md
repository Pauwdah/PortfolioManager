# PortfolioManager

> [!Note]
> This project was initially made for a very spcific page but shall now be going into a more general direction.


## Setup
> [!Important]
> For now, the program needs a specific structure in order to work.


### Needed in HTML
Illustration container: (Images get added and removed here)
```html
<div id="IllustrationContainer"></div>
```
Filter buttons: (all is required, the rest will be generated depending on the tag settings)
```html
<div class="filter_buttons">
       <button data-filter="all">ALL</button>
</div>
```
(Optional) Individual headings:
```html
<h3 class="filterHeading"></h3>
```

### Needed JavaScript files:
If you chose to not use individual headings, just remove the parts starting with "-".

```js
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


> [!Tip]
> Illustrations will have id "ill-1; ill-2" and so on. <br>
> Illustrations will always have class "item" + any amount of selected tags as class for sorting.

## Usage
<p>
When first opening the programm you will get prompted to input you sftp credentials. <br>
Host & Username are automatically encrypted & stored in your user registry. <br>
You can also chose to encrypted & stored your password in your registry. <br> <br>
If you missspelled your password you can try again on Login > Login or just restart the program. 
</p>
<p>
Once loaded in, you can add Images, aswell as change their description (alt text). <br>
You can add tags from Edit > Manage Filter and chose which image gets which tag in the main window. 
</p>

> [!Note]
> The numbers on the top left of each image dont do anything right now they are for sorting later on. <br>
<p>
Once finished dont forget to Publish on File > Publish 
</p>

> [!Note]
> If you already have images in the Illustration Container & they are currently not in the programs files, they will get downloaded the next time you open the program.
