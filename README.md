# Fimfic Epub Camo

Epub files downloaded from Fimfiction have their external images proxied via the `camo.fimfiction.net` domain.
That breaks them in, for example, Calibre when trying to embed external resources. This simple application unpacks
the given `.epub` file, strips the proxy from all images, and repackages the `.epub` with a given suffix.

## Usage

**→ When asked `Source file:`**    
Enter the full, absolute path to the `.epub` file, for example `C://Documents/CoolBook.epub`.

**→ When asked `Output file suffix:`**    
Enter the suffix for the file name to differentiate from the original. For example, suffix `new` will result in the
output file being `C://Documents/CoolBook-new.epub`.

**→ Output**    
The resulting file is created next to the source file.

## Roadmap

- [ ] An option to download images and embed them within the epub so Calibre isn't needed
- [ ] An option to make the downloaded images indexed grayscale to save on space, since ebook readers don't have colour displays anyway
