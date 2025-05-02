import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import Login from './pages/Login'
import Register from './pages/Register'
import Dashboard from './pages/Dashboard'
import { Toaster} from "sonner";

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route path="/dashboard" element={<Dashboard />} />
                <Route path="*" element={<div className="p-4 text-center">404 Not Found</div>} />
            </Routes>
            <div className="bg-red-500 text-white p-4 rounded-lg">Tailwind test</div>
            <Toaster richColors position="top-center" />
        </Router>
    )
}

export default App