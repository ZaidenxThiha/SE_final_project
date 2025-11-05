# Product Images Folder

This folder stores product images uploaded through the admin panel.

## Uploading Images

When creating or editing a product in the Admin panel:
1. Select one or more image files
2. Images will be automatically saved to this folder with unique filenames
3. Multiple images can be uploaded for each product

## Image Format

- Images are stored with unique filenames: `{GUID}_{original-filename}`
- Supported formats: All standard image formats (JPEG, PNG, GIF, etc.)
- Images are accessible via: `/images/products/{filename}`

## Placeholder Image

If a product doesn't have an image, a placeholder image should be placed at:
`/images/products/placeholder.jpg`

You can create a simple placeholder image or use any default product image.

