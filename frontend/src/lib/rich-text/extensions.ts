import StarterKit from "@tiptap/starter-kit";
import { TextStyle } from "@tiptap/extension-text-style";
import TextAlign from "@tiptap/extension-text-align";
import { FontSize } from "./font-size";
import { CustomImage } from "./custom-image";
import { TabExtension } from "./tab-extension";
import {
  Subscript,
  Superscript,
  TextColor,
  Highlight,
} from "./custom-marks";

export const richTextExtensions = [
  StarterKit.configure({
    heading: { levels: [1, 2, 3, 4] },
    codeBlock: {
      HTMLAttributes: {
        class: "rounded-none bg-neutral-900 text-neutral-100 p-4 font-mono text-sm my-4 overflow-x-auto",
      },
    },
    code: {
      HTMLAttributes: {
        class: "rounded bg-erp-surface-subtle px-1.5 py-0.5 font-mono text-xs text-erp-navy-800 dark:text-erp-navy-200",
      },
    },
    link: {
      openOnClick: false,
      autolink: true,
      linkOnPaste: true,
      HTMLAttributes: {
        class: "font-medium text-erp-navy-800 dark:text-erp-navy-200 underline underline-offset-2",
      },
    },
    underline: {
      HTMLAttributes: {
        class: "underline underline-offset-2",
      },
    },
  }),
  CustomImage.configure({
    HTMLAttributes: {
      class: "rounded-none max-w-full h-auto my-4 transition-all",
    },
  }),
  TextAlign.configure({
    types: ["heading", "paragraph"],
  }),
  TextStyle,
  FontSize,
  TextColor,
  Highlight.configure({ multicolor: true }),
  Subscript,
  Superscript,
  TabExtension,
];
