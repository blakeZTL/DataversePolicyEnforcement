import { nodeResolve } from "@rollup/plugin-node-resolve";
import typescript from "@rollup/plugin-typescript";

export default {
  input: "./src/index.ts",
  output: {
    file: "./dist/bundle.js",
    format: "cjs",
    name: "bundle",
  },
  plugins: [nodeResolve(), typescript()],
};
