import { api } from './api';
async function requisitar(config) { const resposta = await api.request(config); return resposta.data?.data; }
export const autenticacaoApi = {
  csrf: () => requisitar({ method: 'get', url: '/Autenticacao/csrf' }),
  entrar: (dados) => requisitar({ method: 'post', url: '/Autenticacao/login', data: dados }),
  sair: () => requisitar({ method: 'post', url: '/Autenticacao/logout' }),
  eu: () => requisitar({ method: 'get', url: '/Autenticacao/eu' }),
};
export const plataformaApi = {
  listar: () => requisitar({ method: 'get', url: '/Plataforma' }),
  obter: (id) => requisitar({ method: 'get', url: `/Plataforma/${id}` }),
  inserir: (dados) => requisitar({ method: 'post', url: '/Plataforma', data: dados }),
  atualizar: (id, dados) => requisitar({ method: 'put', url: `/Plataforma/${id}`, data: dados }),
  excluir: (id) => requisitar({ method: 'delete', url: `/Plataforma/${id}` }),
};
export const generoApi = { listar: () => requisitar({ method: 'get', url: '/Genero' }) };
export const condicaoJogoApi = { listar: () => requisitar({ method: 'get', url: '/CondicaoJogo' }) };
export const jogoApi = {
  listar: (params) => requisitar({ method: 'get', url: '/Jogo', params }),
  obter: (id) => requisitar({ method: 'get', url: `/Jogo/${id}` }),
  inserir: (dados) => requisitar({ method: 'post', url: '/Jogo', data: dados }),
  atualizar: (id, dados) => requisitar({ method: 'put', url: `/Jogo/${id}`, data: dados }),
  excluir: (id) => requisitar({ method: 'delete', url: `/Jogo/${id}` }),
};
