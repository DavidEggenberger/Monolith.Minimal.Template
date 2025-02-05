module.exports = {
    content: ["./**/*.{razor,html,cshtml}"],
    theme: {
        colors: {
            'white': '#FFFFFF',
            'black': '#000000',
            'gray-50': 'rgb(249 250 251)',
            'gray-100': 'rgb(243 244 246)',
            'gray-200': 'rgb(229 231 235)',
            'gray-300': 'rgb(209 213 219)',
            'gray-400': 'rgb(156 163 175)',
            'gray-500': 'rgb(144 153 168)',
            'gray-700': '#374151',
            'white-200': 'rgba(255, 255, 255, 0.2)',
            'white-300': 'rgba(255, 255, 255, 0.3)',
            'white-500': 'rgba(255, 255, 255, 0.5)',
            'white-600': 'rgba(255, 255, 255, 0.6)',
            'white-700': 'rgba(255, 255, 255, 0.7)',
            'white-900': 'rgba(255, 255, 255, 0.9)',
            'primary-50': '#f0faff',
            'primary-100': '#e0f5fe',
            'primary-200': '#b9e9fe',
            'primary-300': '#7cd9fd',
            'primary-400': '#36c7fa',
            'primary-500': '#0cb1ed',
            'primary-600': '#008ec9',
            'primary-700': '#0170a3',
            'primary-800': '#065f86',
            'secondary-100': 'rgb(255, 229, 210, 0.6)',
            'secondary-500': '#FF730F',
            'danger': '#ef4444',
            'danger-600': 'rgba(220, 38, 38, 1)',
            'danger-500': 'rgba(239, 68, 68, 1)',
        },
        screens: {
            'xs': { 'raw': '(min-width: 0px)' },
            'sm': { 'raw': '(min-width: 480px)' },
            'md': { 'raw': '(min-width: 768px)' },
            'lg': { 'raw': '(min-width: 1024px)' },
            'xl': { 'raw': '(min-width: 1280px)' },
            '2xl': { 'raw': '(min-width: 1400px)' }
        },
        gridTemplateColumns: {
            'auto-fit-200': 'repeat(auto-fill, minmax(200px, 1fr))',
        }
    },
    plugins: [],
    safelist: [
        'ml-4'
    ]
}