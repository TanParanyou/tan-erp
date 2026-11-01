//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'supplier_invoice_order_ref_response.g.dart';

/// SupplierInvoiceOrderRefResponse
///
/// Properties:
/// * [id] 
/// * [number] 
/// * [status] 
@BuiltValue()
abstract class SupplierInvoiceOrderRefResponse implements Built<SupplierInvoiceOrderRefResponse, SupplierInvoiceOrderRefResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'status')
  String? get status;

  SupplierInvoiceOrderRefResponse._();

  factory SupplierInvoiceOrderRefResponse([void updates(SupplierInvoiceOrderRefResponseBuilder b)]) = _$SupplierInvoiceOrderRefResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SupplierInvoiceOrderRefResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SupplierInvoiceOrderRefResponse> get serializer => _$SupplierInvoiceOrderRefResponseSerializer();
}

class _$SupplierInvoiceOrderRefResponseSerializer implements PrimitiveSerializer<SupplierInvoiceOrderRefResponse> {
  @override
  final Iterable<Type> types = const [SupplierInvoiceOrderRefResponse, _$SupplierInvoiceOrderRefResponse];

  @override
  final String wireName = r'SupplierInvoiceOrderRefResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SupplierInvoiceOrderRefResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.number != null) {
      yield r'number';
      yield serializers.serialize(
        object.number,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SupplierInvoiceOrderRefResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SupplierInvoiceOrderRefResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'number':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.number = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SupplierInvoiceOrderRefResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SupplierInvoiceOrderRefResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


