import { reactive, type App } from "vue";
import {
  QueryClient,
  VueQueryPlugin,
  type VueQueryPluginOptions,
} from "@tanstack/vue-query";

export default (app: App) => {
  const myClient = new QueryClient();
  const vueQueryPluginOptions: VueQueryPluginOptions = {
    queryClient: myClient,
  };
  app.use(VueQueryPlugin, vueQueryPluginOptions);
};
