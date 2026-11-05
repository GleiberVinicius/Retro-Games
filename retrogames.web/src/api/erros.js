export function obterMensagemErro(erro) { return erro?.response?.data?.errorMessage || 'Não foi possível concluir a operação.'; }
export function obterErrosDosCampos(erro) { return erro?.response?.data?.responseCode === 3 ? erro.response.data.data || {} : {}; }
