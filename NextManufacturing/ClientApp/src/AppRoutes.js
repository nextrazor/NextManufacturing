import ApiAuthorzationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { default as FetchResources } from "./components/resources/FetchResources";
import { default as FetchCalendarStates } from "./components/calendarStates/FetchCalendarStates"
import { default as FetchCalendarTemplates } from "./components/calendarTemplates/FetchCalendarTemplates"
import { Home } from "./components/Home";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/fetch-data',
    requireAuth: true,
    element: <FetchData />
  },
  {
    path: '/fetch-resources',
    element: <FetchResources />
  },
  {
    path: '/fetch-calendar-states',
    element: <FetchCalendarStates />
  },
  {
    path: '/fetch-calendar-templates',
    element: <FetchCalendarTemplates />
  },
  ...ApiAuthorzationRoutes
];

export default AppRoutes;
