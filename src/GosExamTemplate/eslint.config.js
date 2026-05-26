const js = require('@eslint/js');
const globals = require('globals');

module.exports = [
    {
        ignores: ['wwwroot/lib/**', 'bin/**', 'obj/**', 'node_modules/**'],
    },
    js.configs.recommended,
    {
        languageOptions: {
            ecmaVersion: 2022,
            sourceType: 'script',
            globals: {
                ...globals.browser,
                ...globals.jquery,
                bootstrap: 'readonly',
            },
        },
        rules: {
            'no-console': ['warn', { allow: ['error', 'warn'] }],
            'no-unused-vars': ['error', { argsIgnorePattern: '^_' }],
            'prefer-const': 'error',
            semi: ['error', 'always'],
            quotes: ['error', 'single', { avoidEscape: true }],
            'eol-last': ['error', 'always'],
            'no-trailing-spaces': 'error',
        },
    },
];
