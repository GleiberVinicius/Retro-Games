import axios from 'axios';

export const api = axios.create({ baseURL: '/api', withCredentials: true, headers: { 'Content-Type': 'application/json' } });
function lerCookie(nome) { return document.cookie.split('; ').find((item) => item.startsWith(`${nome}=`))?.split('=').slice(1).join('='); }
api.interceptors.request.use((config) => {
  if (['post', 'put', 'delete'].includes(config.method?.toLowerCase())) {
    const token = lerCookie('XSRF-TOKEN');
    if (token) config.headers['X-CSRF-TOKEN'] = decodeURIComponent(token);
  }
  return config;
});
api.interceptors.response.use((res) => res, async (erro) => {
  const config = erro.config;
  if (erro.response?.status === 401) window.dispatchEvent(new Event('sessao:expirada'));
  if (erro.response?.status === 400 && erro.response?.headers?.['x-csrf-invalido'] === 'true' && config && ['post', 'put', 'delete'].includes(config.method?.toLowerCase()) && !config._tentouRenovarCsrf) {
    config._tentouRenovarCsrf = true;
    try { await api.get('/Autenticacao/csrf'); return api(config); } catch { /* preserva o erro original */ }
  }
  return Promise.reject(erro);
});
