import { type RouteConfig, index, layout, route } from "@react-router/dev/routes";

export default [
  layout("routes/layout.tsx", [
    index("routes/index.tsx"),
    route("perioder", "routes/perioder.tsx"),
    route("organisering", "routes/organisering.tsx"),
    route("adgangsstyring", "routes/adgangsstyring.tsx"),
    route("personale", "routes/personale.tsx"),
  ]),
] satisfies RouteConfig;