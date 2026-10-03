import js from "@eslint/js";
import globals from "globals";
import tseslint from "typescript-eslint";
import pluginReact from "eslint-plugin-react";
import reactHooks from "eslint-plugin-react-hooks";
import { defineConfig, globalIgnores } from "eslint/config";

export default defineConfig([
  // Don't lint build output
  globalIgnores(["dist"]),
  { files: ["**/*.{js,mjs,cjs,ts,mts,cts,jsx,tsx}"], plugins: { js }, extends: ["js/recommended"], languageOptions: { globals: globals.browser } },
  tseslint.configs.recommended,
  pluginReact.configs.flat.recommended,
  // React 17+ JSX transform: no need to import React in every file
  pluginReact.configs.flat["jsx-runtime"],
  // Rules of Hooks + exhaustive useEffect dependencies
  reactHooks.configs.flat.recommended,
  { settings: { react: { version: "detect" } } },
]);
