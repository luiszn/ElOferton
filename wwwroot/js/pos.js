(function () {
    'use strict';

    var carrito = [];

    function init() {
        var form = document.getElementById('posForm');
        if (!form) {
            return;
        }

        var searchInput = document.getElementById('productoBuscar');
        var results = document.getElementById('productosResultados');
        var productosModo = document.getElementById('productosModo');
        var carritoBody = document.getElementById('carritoBody');
        var puntoSelect = document.getElementById('PuntoVentaIdSelect') || document.getElementById('PuntoVentaId');
        var metodoSelect = document.getElementById('MetodoPagoId');
        var porcentajeInput = document.getElementById('PorcentajeDescuento');
        var montoInput = document.getElementById('MontoRecibido');
        var clienteSelect = document.getElementById('ClienteId');
        var clienteOcasional = document.getElementById('ClienteOcasional');
        var subtotalLabel = document.getElementById('subtotalLabel');
        var descuentoLabel = document.getElementById('descuentoLabel');
        var totalLabel = document.getElementById('totalLabel');
        var vueltoLabel = document.getElementById('vueltoLabel');
        var montoGroup = document.getElementById('montoGroup');
        var hiddenContainer = document.getElementById('itemsHidden');
        var timer = null;
        var controladorBusqueda = null;
        var initialData = document.getElementById('posItemsIniciales');

        if (initialData && initialData.textContent.trim()) {
            try {
                carrito = JSON.parse(initialData.textContent).map(function (item) {
                    return {
                        ProductoId: item.productoId ?? item.ProductoId,
                        Codigo: item.codigo ?? item.Codigo,
                        Nombre: item.nombre ?? item.Nombre,
                        Cantidad: item.cantidad ?? item.Cantidad,
                        PrecioUnitario: item.precioUnitario ?? item.PrecioUnitario,
                        StockDisponible: item.stockDisponible ?? item.StockDisponible ?? 999999,
                        Subtotal: item.subtotal ?? item.Subtotal
                    };
                });
            } catch (error) {
                carrito = [];
            }
        }

        function money(valor) {
            return 'S/ ' + valor.toFixed(2);
        }

        function redondear(valor) {
            return Math.round((valor + Number.EPSILON) * 100) / 100;
        }

        function calcular() {
            var subtotal = carrito.reduce(function (suma, item) {
                return suma + item.PrecioUnitario * item.Cantidad;
            }, 0);
            subtotal = redondear(subtotal);

            var porcentaje = parseFloat(porcentajeInput.value) || 0;
            var descuento = redondear(subtotal * porcentaje / 100);
            var total = redondear(subtotal - descuento);
            var monto = parseFloat(montoInput.value);
            var vuelto = metodoRequiereMonto() && !isNaN(monto) ? redondear(Math.max(0, monto - total)) : 0;

            subtotalLabel.textContent = money(subtotal);
            descuentoLabel.textContent = money(descuento);
            totalLabel.textContent = money(total);
            vueltoLabel.textContent = money(vuelto);
            montoGroup.style.display = metodoRequiereMonto() ? '' : 'none';
        }

        function metodoRequiereMonto() {
            var opcion = metodoSelect.options[metodoSelect.selectedIndex];
            return opcion ? opcion.getAttribute('data-requiere-monto') === 'true' : false;
        }

        function sincronizarHidden() {
            hiddenContainer.innerHTML = '';
            carrito.forEach(function (item, index) {
                [
                    ['ProductoId', item.ProductoId],
                    ['Codigo', item.Codigo],
                    ['Nombre', item.Nombre],
                    ['Cantidad', item.Cantidad],
                    ['PrecioUnitario', item.PrecioUnitario],
                    ['StockDisponible', item.StockDisponible],
                    ['Subtotal', redondear(item.PrecioUnitario * item.Cantidad)]
                ].forEach(function (par) {
                    var input = document.createElement('input');
                    input.type = 'hidden';
                    input.name = 'Items[' + index + '].' + par[0];
                    input.value = par[1];
                    hiddenContainer.appendChild(input);
                });
            });
        }

        function render() {
            carritoBody.innerHTML = '';

            if (carrito.length === 0) {
                var filaVacia = document.createElement('tr');
                var celda = document.createElement('td');
                celda.colSpan = 5;
                celda.className = 'empty-state';
                celda.textContent = 'Busque un producto y agréguelo al carrito.';
                filaVacia.appendChild(celda);
                carritoBody.appendChild(filaVacia);
            }

            carrito.forEach(function (item, index) {
                var fila = document.createElement('tr');
                fila.className = 'cart-item';

                var nombre = document.createElement('td');
                var fuerte = document.createElement('strong');
                fuerte.textContent = item.Nombre;
                var pequeno = document.createElement('div');
                pequeno.className = 'text-muted small';
                pequeno.textContent = item.Codigo + ' · ' + item.PrecioUnitario.toFixed(2);
                nombre.appendChild(fuerte);
                nombre.appendChild(pequeno);

                var cantidadCelda = document.createElement('td');
                var cantidad = document.createElement('input');
                cantidad.type = 'number';
                cantidad.min = '1';
                cantidad.max = String(item.StockDisponible);
                cantidad.className = 'qty-input';
                cantidad.value = String(item.Cantidad);
                cantidad.addEventListener('change', function () {
                    var valor = parseInt(cantidad.value, 10);
                    if (isNaN(valor) || valor < 1) {
                        valor = 1;
                    }
                    if (valor > item.StockDisponible) {
                        valor = item.StockDisponible;
                        window.alert('Stock disponible: ' + item.StockDisponible);
                    }
                    item.Cantidad = valor;
                    cantidad.value = String(valor);
                    render();
                });
                cantidadCelda.appendChild(cantidad);

                var subtotalCelda = document.createElement('td');
                subtotalCelda.className = 'text-end fw-semibold';
                subtotalCelda.textContent = money(redondear(item.PrecioUnitario * item.Cantidad));

                var acciones = document.createElement('td');
                acciones.className = 'text-end';
                var boton = document.createElement('button');
                boton.type = 'button';
                boton.className = 'btn btn-sm btn-outline-danger';
                boton.setAttribute('aria-label', 'Eliminar producto');
                boton.innerHTML = '<i class="bi bi-trash3"></i>';
                boton.addEventListener('click', function () {
                    carrito.splice(index, 1);
                    render();
                });
                acciones.appendChild(boton);

                fila.appendChild(nombre);
                fila.appendChild(cantidadCelda);
                fila.appendChild(subtotalCelda);
                fila.appendChild(acciones);
                carritoBody.appendChild(fila);
            });

            sincronizarHidden();
            calcular();
        }

        function agregar(producto) {
            var existente = carrito.find(function (item) {
                return item.ProductoId === producto.productoId;
            });

            if (existente) {
                if (existente.Cantidad >= existente.StockDisponible) {
                    window.alert('No hay más stock disponible para ' + existente.Nombre);
                    return;
                }
                existente.Cantidad += 1;
            } else {
                carrito.push({
                    ProductoId: producto.productoId,
                    Codigo: producto.codigo,
                    Nombre: producto.nombre,
                    PrecioUnitario: producto.precio,
                    StockDisponible: producto.stock,
                    Cantidad: 1
                });
            }

            render();
        }

        function buscar(terminoBusqueda, despuesDeBuscar) {
            var termino = terminoBusqueda === undefined ? searchInput.value.trim() : terminoBusqueda;
            var puntoVentaId = parseInt(puntoSelect.value, 10);
            if (!puntoVentaId) {
                results.innerHTML = '<div class="empty-state small">Seleccione un punto de venta.</div>';
                return;
            }

            if (productosModo) {
                productosModo.textContent = termino ? 'Resultados' : 'Sugeridos';
            }

            if (controladorBusqueda) {
                controladorBusqueda.abort();
            }
            controladorBusqueda = new AbortController();

            var url = form.getAttribute('data-search-url')
                + '?termino=' + encodeURIComponent(termino)
                + '&puntoVentaId=' + puntoVentaId;

            fetch(url, {
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                signal: controladorBusqueda.signal
            })
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error('Error al buscar productos');
                    }
                    return response.json();
                })
                .then(function (productos) {
                    results.innerHTML = '';
                    if (!productos.length) {
                        var mensaje = document.createElement('div');
                        mensaje.className = 'empty-state small';
                        mensaje.textContent = termino
                            ? 'No se encontraron productos con ese nombre, código, categoría o marca.'
                            : 'No hay productos sugeridos con stock en este ambiente.';
                        results.appendChild(mensaje);
                        if (despuesDeBuscar) {
                            despuesDeBuscar(productos);
                        }
                        return;
                    }

                    productos.forEach(function (producto) {
                        var boton = document.createElement('button');
                        boton.type = 'button';
                        boton.className = 'pos-result-item w-100 text-start';
                        var info = document.createElement('div');
                        var nombre = document.createElement('strong');
                        nombre.textContent = producto.nombre;
                        var detalle = document.createElement('div');
                        detalle.className = 'text-muted small';
                        detalle.textContent = producto.codigo + ' · ' + producto.categoria + ' · ' + (producto.marca || 'Sin marca');
                        info.appendChild(nombre);
                        info.appendChild(detalle);

                        var datos = document.createElement('div');
                        datos.className = 'text-end';
                        var precio = document.createElement('div');
                        precio.className = 'fw-bold text-purple';
                        precio.textContent = money(producto.precio);
                        var stock = document.createElement('div');
                        stock.className = 'text-muted small';
                        stock.textContent = 'Stock: ' + producto.stock;
                        datos.appendChild(precio);
                        datos.appendChild(stock);

                        boton.appendChild(info);
                        boton.appendChild(datos);
                        boton.addEventListener('click', function () {
                            agregar(producto);
                            searchInput.value = '';
                            buscar('');
                            searchInput.focus();
                        });
                        results.appendChild(boton);
                    });

                    if (despuesDeBuscar) {
                        despuesDeBuscar(productos);
                    }
                })
                .catch(function (error) {
                    if (error && error.name === 'AbortError') {
                        return;
                    }
                    results.innerHTML = '<div class="alert alert-danger small">No se pudo realizar la búsqueda.</div>';
                });
        }

        searchInput.addEventListener('input', function () {
            window.clearTimeout(timer);
            if (!searchInput.value.trim()) {
                buscar('');
                return;
            }
            timer = window.setTimeout(buscar, 150);
        });

        searchInput.addEventListener('keydown', function (event) {
            if (event.key !== 'Enter' && event.keyCode !== 13) {
                return;
            }

            event.preventDefault();
            window.clearTimeout(timer);
            var codigo = searchInput.value.trim();
            if (!codigo) {
                buscar('');
                return;
            }

            buscar(codigo, function (productos) {
                var exacto = productos.find(function (producto) {
                    return producto.codigo.toUpperCase() === codigo.toUpperCase();
                });
                if (!exacto) {
                    return;
                }

                agregar(exacto);
                searchInput.value = '';
                buscar('');
                searchInput.focus();
            });
        });

        puntoSelect.addEventListener('change', function () {
            carrito = [];
            searchInput.value = '';
            render();
            buscar('');
        });

        metodoSelect.addEventListener('change', calcular);
        porcentajeInput.addEventListener('input', calcular);
        montoInput.addEventListener('input', calcular);

        form.addEventListener('submit', function (event) {
            if (carrito.length === 0) {
                event.preventDefault();
                window.alert('Agregue al menos un producto al carrito.');
                return;
            }

            if (!clienteSelect.value && !clienteOcasional.value.trim()) {
                event.preventDefault();
                window.alert('Seleccione un cliente o ingrese un cliente ocasional.');
                return;
            }

            if (metodoRequiereMonto()) {
                var monto = parseFloat(montoInput.value);
                var total = parseFloat(totalLabel.textContent.replace(/[^\d.-]/g, ''));
                if (isNaN(monto) || monto < total) {
                    event.preventDefault();
                    window.alert('El monto recibido debe ser mayor o igual al total.');
                }
            }
        });

        render();
        buscar('');
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();
