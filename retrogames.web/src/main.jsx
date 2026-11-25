import React from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import './index.css';
import { AutenticacaoProvider } from './contexts/AutenticacaoProvider';
import App from './App';

createRoot(document.getElementById('root')).render(
  <React.StrictMode><BrowserRouter basename={import.meta.env.BASE_URL}><AutenticacaoProvider><App /></AutenticacaoProvider></BrowserRouter></React.StrictMode>,
);
