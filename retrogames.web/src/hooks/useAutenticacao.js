import { useContext } from 'react';
import { autenticacaoContext } from '../contexts/autenticacaoContext';
export function useAutenticacao() { return useContext(autenticacaoContext); }
