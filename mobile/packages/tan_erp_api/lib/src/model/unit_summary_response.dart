//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/localized_text_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'unit_summary_response.g.dart';

/// UnitSummaryResponse
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [symbol] 
/// * [name] 
@BuiltValue()
abstract class UnitSummaryResponse implements Built<UnitSummaryResponse, UnitSummaryResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'symbol')
  String? get symbol;

  @BuiltValueField(wireName: r'name')
  LocalizedTextResponse? get name;

  UnitSummaryResponse._();

  factory UnitSummaryResponse([void updates(UnitSummaryResponseBuilder b)]) = _$UnitSummaryResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UnitSummaryResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UnitSummaryResponse> get serializer => _$UnitSummaryResponseSerializer();
}

class _$UnitSummaryResponseSerializer implements PrimitiveSerializer<UnitSummaryResponse> {
  @override
  final Iterable<Type> types = const [UnitSummaryResponse, _$UnitSummaryResponse];

  @override
  final String wireName = r'UnitSummaryResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UnitSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.symbol != null) {
      yield r'symbol';
      yield serializers.serialize(
        object.symbol,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType(LocalizedTextResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UnitSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UnitSummaryResponseBuilder result,
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
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'symbol':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.symbol = valueDes;
          break;
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(LocalizedTextResponse),
          ) as LocalizedTextResponse?;
          if (valueDes == null) continue;
          result.name.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UnitSummaryResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UnitSummaryResponseBuilder();
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


