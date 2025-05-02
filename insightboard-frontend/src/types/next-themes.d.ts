declare module "next-themes/dist/types" {
    import { ReactNode } from "react"

    export interface ThemeProviderProps {
        children: ReactNode
        attribute?: string
        defaultTheme?: string
        enableSystem?: boolean
        storageKey?: string
    }
}
