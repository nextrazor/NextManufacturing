import React, {Component} from 'react';
import {Route, Routes} from 'react-router-dom';
import AppRoutes from './AppRoutes';
import AuthorizeRoute from './components/api-authorization/AuthorizeRoute';
import {LocalLayout} from './components/Layout';
import {Suspense} from 'react';
import { default as FetchCalendarTemplatesEditor } from "./components/calendarTemplates/editor/FetchCalendarTemplatesEditor"
import './custom.css';

export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <Suspense fallback="...is loading">
                <LocalLayout>
                    <Routes>
                    <Route
                                              path="/fetch-calendar-templates-editor/:template"
                                              element=<FetchCalendarTemplatesEditor/>
                                              //render={({ match }) => <FetchCalendarTemplatesEditor template={match.params.template} />}
                                            />
                        {AppRoutes.map((route, index) => {
                            const {element, requireAuth, ...rest} = route;
                            return <Route key={index} {...rest} element={requireAuth ?
                                <AuthorizeRoute {...rest} element={element}/> : element}/>;
                        })}
                        
                    </Routes>
                </LocalLayout>
            </Suspense>
        );
    }
}



//  {
//      path: '/fetch-calendar-templates-editor/:template',
//      element: <FetchCalendarTemplatesEditor />
//  },