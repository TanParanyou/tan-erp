import type thMessages from "./messages/th.json";

declare global {
  // Use type safe message keys with next-intl
  type Messages = typeof thMessages;
  interface IntlMessages extends Messages {}
}
